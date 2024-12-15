#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Pricing;
using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.Services.Converters;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Commands.BalancePayment;

// ReSharper disable always UnusedType.Global
public record BalancePaymentCommand : IRequest<Result>
{
    public required int RegistrationToExhibitionId { get; init; }
    public required string WebAddress { get; init; }
    public required string RootPath { get; init; }
}

public class BalancePaymentPaymentCommandValidator : AbstractValidator<BalancePaymentCommand>
{
    public BalancePaymentPaymentCommandValidator()
    {
        RuleFor(x => x.RegistrationToExhibitionId).ForeignKeyValidator();
        RuleFor(x => x.WebAddress).NotEmpty();
        RuleFor(x => x.RootPath).NotEmpty();
    }
}

public class BalancePaymentCommandHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    IUser user,
    IInvoiceSenderService invoiceSenderService,
    IPricingFacade pricingFacade)
    : IRequestHandler<BalancePaymentCommand, Result>
{
    public async Task<Result> Handle(BalancePaymentCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId),
            Operations.DoOrganizationAdminStuff);
        Guard.Against.UnAuthorized(authorizationResult);

        RegistrationToExhibitionDto registrationToExhibitionDto =
            await registrationToExhibitionService.GetRegistrationToExhibitionById(request.RegistrationToExhibitionId,
                cancellationToken);

        RegistrationToExhibitionPrice price =
            await pricingFacade.GetPrice(request.RegistrationToExhibitionId, cancellationToken);

        Currency? currency = registrationToExhibitionDto.PaymentInfo?.Currency;
        Guard.Against.Null(currency, nameof(currency));
        Result result = await registrationToExhibitionService.BalanceThePayment(request.RegistrationToExhibitionId,
            price.GTotalPrice.GetPriceForCurrency(currency.Value),
            cancellationToken);
        await invoiceSenderService.SendPaymentConfirmationInvoiceToMail(request.RegistrationToExhibitionId,
            request.WebAddress, request.RootPath);
        return result;
    }
}
