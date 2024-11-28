#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Services.Converters;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Commands.FinishDelayedPayment;

// ReSharper disable always UnusedType.Global
public record FinishDelayedPaymentCommand : IRequest<Result>
{
    public required int RegistrationToExhibitionId { get; init; }
    public required string WebAddress { get; init; }
    public required string RootPath { get; init; }
}

public class FinishDelayedPaymentPaymentCommandValidator : AbstractValidator<FinishDelayedPaymentCommand>
{
    public FinishDelayedPaymentPaymentCommandValidator()
    {
        RuleFor(x => x.RegistrationToExhibitionId).ForeignKeyValidator();
        RuleFor(x => x.WebAddress).NotEmpty().MaximumLength(255);
        RuleFor(x => x.RootPath).NotEmpty().MaximumLength(255);
    }
}

public class FinishDelayedPaymentCommandHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    IUser user,
    IInvoiceSenderService invoiceSenderService)
    : IRequestHandler<FinishDelayedPaymentCommand, Result>
{
    public async Task<Result> Handle(FinishDelayedPaymentCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizeAsync = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId),
            Operations.DoOrganizationAdminStuff);
        Guard.Against.UnAuthorized(authorizeAsync);

        Result result = await registrationToExhibitionService.FinishDelayedPayment(request.RegistrationToExhibitionId,
            cancellationToken);

        if (result.IsSuccess)
        {
            await invoiceSenderService.SendPaymentConfirmationInvoiceToMail(request.RegistrationToExhibitionId,
                request.WebAddress, request.RootPath);
        }

        return result;
    }
}
