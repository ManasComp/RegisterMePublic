#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitors;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.Pricing;
using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;
using Stripe.Checkout;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Commands.StartOnlinePayment;

// ReSharper disable always UnusedType.Global
public record StartOnlinePaymentCommand : IRequest<Result<SessionDto>>
{
    public required string SuccessUrl { get; init; } = null!;
    public required string CancelUrl { get; init; } = null!;
    public required int RegistrationToExhibitionId { get; init; }
    public required Currency Currency { get; init; }
}

public class StartOnlinePaymentCommandValidator : AbstractValidator<StartOnlinePaymentCommand>
{
    public StartOnlinePaymentCommandValidator()
    {
        RuleFor(v => v.RegistrationToExhibitionId).GreaterThan(0);
        RuleFor(v => v.SuccessUrl).NotEmpty().MaximumLength(255);
        RuleFor(v => v.CancelUrl).NotEmpty().MaximumLength(255);
    }
}

public class StartOnlinePaymentCommandHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    IUser user,
    IStripeInvoiceBuilder stripeInvoiceBuilder,
    IExhibitorService exhibitorService,
    IPricingFacade pricingFacade,
    SessionService sessionService) : IRequestHandler<StartOnlinePaymentCommand, Result<SessionDto>>
{
    public async Task<Result<SessionDto>> Handle(StartOnlinePaymentCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId),
                Operations.OnlyOwnerCanDo);
        Guard.Against.UnAuthorized(authorizationResult);

        ExhibitorAndUserDto exhibitor =
            await exhibitorService.GetExhibitorByRegistrationToExhibitionId(request.RegistrationToExhibitionId,
                cancellationToken);

        SessionCreateOptions options = new()
        {
            SuccessUrl = request.SuccessUrl,
            CancelUrl = request.CancelUrl,
            LineItems = await stripeInvoiceBuilder.GetInvoice(request.RegistrationToExhibitionId, request.Currency),
            Mode = "payment",
            CustomerEmail = exhibitor.Email
        };

        Session? session =
            await sessionService.CreateAsync(options, cancellationToken: cancellationToken);
        Guard.Against.Null(session);

        RegistrationToExhibitionPrice price =
            await pricingFacade.GetPrice(request.RegistrationToExhibitionId, cancellationToken);
        Result result = await registrationToExhibitionService.StartOnlinePayment(request.RegistrationToExhibitionId,
            session.Id,
            request.Currency, price.GTotalPrice.GetPriceForCurrency(request.Currency),
            cancellationToken);

        SessionDto sessionObj = new() { Id = session.Id, Url = session.Url };

        return result.IsSuccess ? Result.Success(sessionObj) : Result.Failure<SessionDto>(result.Error);
    }
}
