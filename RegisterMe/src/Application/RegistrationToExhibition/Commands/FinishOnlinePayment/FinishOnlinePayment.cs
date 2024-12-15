#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.Services.Converters;
using RegisterMe.Domain.Common;
using Stripe.Checkout;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Commands.FinishOnlinePayment;

// ReSharper disable always UnusedType.Global
public record FinishOnlinePaymentCommand : IRequest<Result>
{
    public required int RegistrationToExhibitionId { get; init; }
    public required string WebAddress { get; init; }
    public required string RootPath { get; init; }
}

public class FinishOnlinePaymentCommandValidator : AbstractValidator<FinishOnlinePaymentCommand>
{
    public FinishOnlinePaymentCommandValidator()
    {
        RuleFor(x => x.RegistrationToExhibitionId).ForeignKeyValidator();
        RuleFor(x => x.WebAddress).NotEmpty().MaximumLength(255);
        RuleFor(x => x.RootPath).NotEmpty().MaximumLength(255);
    }
}

public class FinishOnlinePaymentCommandHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    IUser user,
    SessionService sessionService,
    IInvoiceSenderService invoiceSenderService)
    : IRequestHandler<FinishOnlinePaymentCommand, Result>
{
    public async Task<Result> Handle(FinishOnlinePaymentCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId), Operations.OnlyOwnerCanDo);
        Guard.Against.UnAuthorized(authorizationResult);

        RegistrationToExhibitionDto registrationToExhibition =
            await registrationToExhibitionService.GetRegistrationToExhibitionById(request.RegistrationToExhibitionId,
                cancellationToken);
        Guard.Against.Null(registrationToExhibition.PaymentInfo?.SessionId);

        Session? session = await sessionService.GetAsync(registrationToExhibition.PaymentInfo!.SessionId,
            cancellationToken: cancellationToken);

        if (!session.PaymentStatus.Equals("paid", StringComparison.CurrentCultureIgnoreCase))
        {
            return Result.Failure(Errors.PaymentIsNotCompletedYetError);
        }

        Result result = await registrationToExhibitionService.FinishOnlinePayment(request.RegistrationToExhibitionId,
            session.Id,
            session.PaymentIntentId, cancellationToken);

        if (result.IsSuccess)
        {
            await invoiceSenderService.SendPaymentConfirmationInvoiceToMail(request.RegistrationToExhibitionId,
                request.WebAddress, request.RootPath);
        }

        return result;
    }
}
