#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Commands.ChangeAdvertisement;

// ReSharper disable always UnusedType.Global
public record ChangeAdvertisementsCommand : IRequest<Result>
{
    public required int AdvertisementId { get; init; }
    public required int RegistrationToExhibitionId { get; init; }
}

public class ChangeAdvertisementsCommandValidator : AbstractValidator<ChangeAdvertisementsCommand>
{
    public ChangeAdvertisementsCommandValidator()
    {
        RuleFor(v => v.AdvertisementId).ForeignKeyValidator();
        RuleFor(v => v.RegistrationToExhibitionId).ForeignKeyValidator();
    }
}

public class ChangeAdvertisementsCommandHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IUser user,
    IAuthorizationService authorizationService) : IRequestHandler<ChangeAdvertisementsCommand, Result>
{
    public async Task<Result> Handle(ChangeAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId),
                Operations.Update);
        Guard.Against.UnAuthorized(authorizationResult);
        Result result = await registrationToExhibitionService.ChangeAdvertisement(request.AdvertisementId,
            request.RegistrationToExhibitionId, cancellationToken);
        return result;
    }
}
