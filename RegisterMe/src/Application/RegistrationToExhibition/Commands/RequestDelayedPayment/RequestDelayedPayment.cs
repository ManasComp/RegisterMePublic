#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Pricing;
using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.Services.Converters;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Commands.RequestDelayedPayment;

// ReSharper disable always UnusedType.Global
public record RequestDelayedPaymentCommand : IRequest<Result>
{
    public required int RegistrationToExhibitionId { get; init; }
    public required PaymentType PaymentType { get; init; }
    public required Currency Currency { get; init; }
    public required string WebAddress { get; init; }
    public required string RootPath { get; init; }
}

public class RequestDelayedPaymentCommandValidator : AbstractValidator<RequestDelayedPaymentCommand>
{
    public RequestDelayedPaymentCommandValidator()
    {
        RuleFor(v => v.RegistrationToExhibitionId).ForeignKeyValidator();
        RuleFor(v => v.WebAddress).NotEmpty().MaximumLength(255);
        RuleFor(v => v.RootPath).NotEmpty().MaximumLength(255);
    }
}

public class RequestDelayedPaymentCommandHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    IUser user,
    IInvoiceSenderService invoiceSenderService,
    IPricingFacade pricingFacade) : IRequestHandler<RequestDelayedPaymentCommand, Result>
{
    public async Task<Result> Handle(RequestDelayedPaymentCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId),
                Operations.OnlyOwnerCanDo);
        Guard.Against.UnAuthorized(authorizationResult);

        RegistrationToExhibitionPrice price =
            await pricingFacade.GetPrice(request.RegistrationToExhibitionId, cancellationToken);
        Result result = await registrationToExhibitionService.RequestDelayedPayment(request.RegistrationToExhibitionId,
            request.PaymentType, request.Currency, price.GTotalPrice.GetPriceForCurrency(request.Currency),
            cancellationToken);

        if (result.IsSuccess)
        {
            await invoiceSenderService.SendRegistrationConfirmationInvoiceToMail(request.RegistrationToExhibitionId,
                request.WebAddress, request.RootPath);
        }

        return result;
    }
}
