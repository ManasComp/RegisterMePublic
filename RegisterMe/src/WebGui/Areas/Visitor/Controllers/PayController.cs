#region

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetCagesStatistics;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionById;
using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.Pricing.Enums;
using RegisterMe.Application.Pricing.Queries.GetAvailablePaymentTypes;
using RegisterMe.Application.Pricing.Queries.GetBeneficiaryMessage;
using RegisterMe.Application.Pricing.Queries.GetPrice;
using RegisterMe.Application.RegistrationToExhibition.Commands.BalancePayment;
using RegisterMe.Application.RegistrationToExhibition.Commands.FinishDelayedPayment;
using RegisterMe.Application.RegistrationToExhibition.Commands.FinishOnlinePayment;
using RegisterMe.Application.RegistrationToExhibition.Commands.RequestDelayedPayment;
using RegisterMe.Application.RegistrationToExhibition.Commands.StartOnlinePayment;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;
using RegisterMe.Infrastructure;
using WebGui.Areas.Visitor.Models;

#endregion

namespace WebGui.Areas.Visitor.Controllers;

[Area(Areas.Visitor)]
public class PayController(
    IAuthorizationService authorizationService,
    IMediator mediator,
    IWebHostEnvironment env)
    : BaseController(authorizationService, mediator)
{
    [HttpGet]
    public async Task<IActionResult> PayInformation(int registrationToExhibitionId)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);
        RegistrationToExhibitionDto registration =
            await SendQuery(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId
            });
        BriefExhibitionDto briefExhibition =
            await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = registration.ExhibitionId });
        RegistrationToExhibitionPrice price =
            await SendQuery(new GetPriceQuery { RegistrationToExhibitionId = registration.Id });
        string message =
            await SendQuery(new GetBeneficiaryMessageQuery { RegistrationToExhibitionId = registrationToExhibitionId });

        bool canPayCzk = price.GTotalPrice.PriceCzk >= 0.1m;
        string qrcodeCzk = new QrCode(briefExhibition.Iban, price.GTotalPrice.PriceCzk, Currency.Czk,
                message, registrationToExhibitionId)
            .GenerateQrCode();

        bool canPayEur = price.GTotalPrice.PriceEur >= 0.1m;
        string qrcodeEur = new QrCode(briefExhibition.Iban, price.GTotalPrice.PriceEur, Currency.Eur,
                message, registrationToExhibitionId)
            .GenerateQrCode();

        List<PaymentTypeWithCurrency> paymentTypes =
            await SendQuery(
                new GetAvailablePaymentTypesQuery { RegistrationToExhibitionId = registrationToExhibitionId });

        PayInformationVm payInformationVm = new()
        {
            CatRegistrationStats =
                await SendQuery(new GetCagesStatisticsQuery
                {
                    RegistrationToExhibitionId = registrationToExhibitionId
                }),
            CanPay = new Dictionary<Currency, bool> { { Currency.Czk, canPayCzk }, { Currency.Eur, canPayEur } },
            PaymentTypes = paymentTypes,
            VariableSymbol = registrationToExhibitionId,
            QrCodes = new Dictionary<Currency, string> { { Currency.Czk, qrcodeCzk }, { Currency.Eur, qrcodeEur } },
            NormalAccount = briefExhibition.BankAccount,
            Iban = briefExhibition.Iban,
            ExhibitionName = briefExhibition.Name,
            ExhibitionId = briefExhibition.Id,
            Message = message,
            RegistrationToExhibitionPrice = price,
            RegistrationToExhibitionId = registrationToExhibitionId,
            CurrencyUserCanPayIn = paymentTypes.Select(x => x.Currency).Distinct().ToList()
        };

        return View(payInformationVm);
    }

    [HttpPost]
    public async Task<IActionResult> PayInPlace(PayModel payModel)
    {
        await SendCommand(
            new RequestDelayedPaymentCommand
            {
                RegistrationToExhibitionId = payModel.RegistrationToExhibitionId,
                PaymentType = PaymentType.PayInPlaceByCache,
                Currency = payModel.Currency,
                WebAddress = GetWebAddress.WebAddress,
                RootPath = env.ContentRootPath
            }, true, "Vaši registraci jsme přijali, zaplatíte na místě");

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> PayByBankTransfer(PayModel payModel)
    {
        await SendCommand(
            new RequestDelayedPaymentCommand
            {
                RegistrationToExhibitionId = payModel.RegistrationToExhibitionId,
                PaymentType = PaymentType.PayByBankTransfer,
                Currency = payModel.Currency,
                WebAddress = GetWebAddress.WebAddress,
                RootPath = env.ContentRootPath
            }, true, "Vaši registraci přijmeme, až obrdžíme platbu");

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> PayOnline(PayModel payModel)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(payModel.RegistrationToExhibitionId),
            Operations.Read);

        Result<SessionDto> session = await SendCommand(new StartOnlinePaymentCommand
        {
            Currency = payModel.Currency,
            RegistrationToExhibitionId = payModel.RegistrationToExhibitionId,
            SuccessUrl =
                $"{GetWebAddress.WebAddress}{Areas.Visitor}/Pay/{nameof(ConfirmOnlinePayment)}?registrationToExhibitionId={payModel.RegistrationToExhibitionId}",
            CancelUrl =
                $"{GetWebAddress.WebAddress}{Areas.Visitor}/Pay/{nameof(PayInformation)}?registrationToExhibitionId={payModel.RegistrationToExhibitionId}"
        });

        Response.Headers.Append("Location", session.Value.Url);
        return new StatusCodeResult(303);
    }

    // as this is a callback from the payment gateway, it must be a GET
    [HttpGet]
    public async Task<IActionResult> ConfirmOnlinePayment(int registrationToExhibitionId)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);

        await SendCommand(
            new FinishOnlinePaymentCommand
            {
                RegistrationToExhibitionId = registrationToExhibitionId,
                WebAddress = GetWebAddress.WebAddress,
                RootPath = env.ContentRootPath
            }, true, "Vaši registraci jsme přijali", "Platba se nezdařila");

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmDelayedPayment(int registrationToExhibitionId)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);
        await SendCommand(
            new FinishDelayedPaymentCommand
            {
                RegistrationToExhibitionId = registrationToExhibitionId,
                WebAddress = GetWebAddress.WebAddress,
                RootPath = env.ContentRootPath
            }, true, "Platba potvrzena");
        return RedirectToAction("Detail", "RegistrationToExhibition",
            new { registrationToExhibitionId });
    }

    [HttpPost]
    public async Task<IActionResult> BalanceThePrice(int registrationToExhibitionId)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);

        await SendCommand(
            new BalancePaymentCommand
            {
                RegistrationToExhibitionId = registrationToExhibitionId,
                WebAddress = GetWebAddress.WebAddress,
                RootPath = env.ContentRootPath
            }, true, "Platba potvrzena");

        return RedirectToAction("Detail", "RegistrationToExhibition",
            new { registrationToExhibitionId });
    }
}
