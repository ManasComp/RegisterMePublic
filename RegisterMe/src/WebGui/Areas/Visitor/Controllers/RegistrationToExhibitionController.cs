#region

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionById;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionCagesInfo;
using RegisterMe.Application.Exhibitions.Queries.HasDrafts;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.Exhibitors.Queries.GetExhibitorById;
using RegisterMe.Application.Exhibitors.Queries.GetExhibitorByUserId;
using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.Pricing.Enums;
using RegisterMe.Application.Pricing.Queries.GetAvailablePaymentTypes;
using RegisterMe.Application.Pricing.Queries.GetPrice;
using RegisterMe.Application.RegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Commands.DeleteTemporaryRegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationsToExhibitionByExhibitorId;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionByExhibitionId;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Application.RegistrationToExhibition.Queries.VerifyEmsByRegistrationToExhibitionId;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;
using RegisterMe.Infrastructure;
using WebGui.Areas.Visitor.Models;

#endregion

namespace WebGui.Areas.Visitor.Controllers;

[Area(Areas.Visitor)]
public class RegistrationToExhibitionController(
    IAuthorizationService authorizationService,
    IMediator mediator,
    UserManager<ApplicationUser> userManager)
    : BaseController(authorizationService, mediator)
{
    [HttpGet]
    public async Task<IActionResult> AdminIndex(int exhibitionId)
    {
        List<RegistrationToExhibitionDto> registrations =
            await SendQuery(new GetRegistrationsToExhibitionByExhibitionIdQuery { ExhibitionId = exhibitionId });
        List<RegistrationToExhibitionAdminIndexModel> models = [];
        BriefExhibitionDto exhibition = await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = exhibitionId });
        foreach (RegistrationToExhibitionDto registration in registrations)
        {
            ExhibitorAndUserDto exhibitor =
                await SendQuery(new GetExhibitorByIdQuery { ExhibitorId = registration.ExhibitorId });
            RegistrationToExhibitionAdminIndexModel model = new()
            {
                ExhibitionName = exhibition.Name,
                Status = registration.OrderStatus().ToString(),
                Name = exhibitor.FirstName,
                Surname = exhibitor.LastName,
                NumberOfCats = registration.CatRegistrationIds.Count,
                IsPaid = registration.PaymentInfo?.PaymentCompletedDate != null,
                MemberNumber = registration.PersonRegistration.MemberNumber,
                RegistrationToExhibitionId = registration.Id,
                Organization = registration.PersonRegistration.Organization,
                IsSend = RegistrationToExhibitionService.IsPaid(registration.PaymentInfo)
            };
            models.Add(model);
        }

        List<ExhibitionCagesInfo> cagesInfo =
            await SendQuery(new GetExhibitionCagesInfoQuery { ExhibitionId = exhibitionId });
        bool hasDrafts = await SendQuery(new HasDraftsQuery { ExhibitionId = exhibitionId });

        AdminIndex adminIndex = new()
        {
            ExhibitionCagesInfos = cagesInfo,
            Registrations = models,
            ExhibitionId = exhibitionId,
            HasDrafts = hasDrafts,
            DeleteAfterHours = exhibition.DeleteNotFinishedRegistrationsAfterHours
        };

        return View(adminIndex);
    }


    [HttpPost]
    public async Task<IActionResult> DeleteTemporaryRegistrations(int exhibitionId)
    {
        await SendCommand(new DeleteTemporaryRegistrationToExhibitionCommand
        {
            WebAddress = GetWebAddress.WebAddress, ExhibitionId = exhibitionId
        });
        return RedirectToAction("AdminIndex", new { exhibitionId });
    }

    [HttpGet]
    public async Task<IActionResult> UserIndex()
    {
        ExhibitorAndUserDto? exhibitor = await SendQuery(new GetExhibitorByUserIdQuery { UserId = GetUserId() });
        if (exhibitor == null)
        {
            TempData["error"] = "Nejste registrován jako vystavovatel";
            return RedirectToAction("Index", "Home");
        }

        List<RegistrationToExhibitionDto> registrations =
            await SendQuery(new GetRegistrationsToExhibitionByExhibitorIdQuery { ExhibitorId = exhibitor.Id });

        List<RegistrationToExhibitionAdminIndexModel> models = [];

        foreach (RegistrationToExhibitionDto registration in registrations)
        {
            BriefExhibitionDto exhibition =
                await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = registration.ExhibitionId });
            ExhibitorAndUserDto exhibitorDto =
                await SendQuery(new GetExhibitorByIdQuery { ExhibitorId = registration.ExhibitorId });
            string status;
            if (registration.OrderStatus() == OrderStatus.PaymentInProgress)
            {
                status = "Čeká na platbu";
            }
            else if (registration.OrderStatus() == OrderStatus.PaymentCompleted)
            {
                status = "Zaplaceno";
            }
            else if (registration.OrderStatus() == OrderStatus.RegisteredWithCats)
            {
                status = "Registrováno s kočkami";
            }
            else if (registration.OrderStatus() == OrderStatus.RegisteredWithoutCats)
            {
                status = "Registrace vystavovatele";
            }
            else
            {
                status = "Zrušeno";
            }

            RegistrationToExhibitionAdminIndexModel model = new()
            {
                ExhibitionName = exhibition.Name,
                Status = status,
                Name = exhibitorDto.FirstName,
                Surname = exhibitorDto.LastName,
                NumberOfCats = registration.CatRegistrationIds.Count,
                MemberNumber = registration.PersonRegistration.MemberNumber,
                IsPaid = registration.PaymentInfo?.PaymentCompletedDate != null,
                RegistrationToExhibitionId = registration.Id,
                Organization = registration.PersonRegistration.Organization,
                IsSend = RegistrationToExhibitionService.IsPaid(registration.PaymentInfo)
            };
            models.Add(model);
        }

        return View(models);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int registrationToExhibitionId)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);

        RegistrationToExhibitionPrice price =
            await SendQuery(new GetPriceQuery { RegistrationToExhibitionId = registrationToExhibitionId });

        RegistrationToExhibitionDto registration =
            await SendQuery(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId
            });
        if (registration.PaymentInfo == null || RegistrationToExhibitionService.IsNotPaid(registration.PaymentInfo))
        {
            return RedirectToAction("PayInformation", "Pay", new { registrationToExhibitionId });
        }

        BriefExhibitionDto briefExhibition =
            await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = registration.ExhibitionId });

        ApplicationUser? user = await userManager.GetUserAsync(User);
        List<PaymentTypeWithCurrency> paymentTypes =
            await SendQuery(
                new GetAvailablePaymentTypesQuery { RegistrationToExhibitionId = registrationToExhibitionId });

        bool isValidEms =
            await SendQuery(new HasValidEmsQuery { RegistrationToExhibitionId = registrationToExhibitionId });

        DetailVm detail = new()
        {
            EmsOk = isValidEms,
            ExhibitionName = briefExhibition.Name,
            ExhibitionId = briefExhibition.Id,
            RegistrationToExhibitionPrice = price,
            RegistrationToExhibitionId = registrationToExhibitionId,
            PaymentCompletedDate = registration.PaymentInfo.PaymentCompletedDate,
            PaymentRequestDate = registration.PaymentInfo.PaymentRequestDate,
            PaymentType = registration.PaymentInfo.PaymentType,
            IsOrganizationAdministrator = user?.OrganizationId == briefExhibition.OrganizationId,
            CurrencyUserCanPayIn = paymentTypes.Select(x => x.Currency).Distinct().ToList(),
            Currency = registration.PaymentInfo.Currency,
            AmountPaid = registration.PaymentInfo?.Amount
        };

        return View(detail);
    }
}
