#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.DeleteAdvertisementById;

// ReSharper disable always UnusedType.Global
public record DeleteAdvertisementCommand : IRequest<Result>
{
    public required int AdvertisementId { get; init; }
}

public class DeleteAdvertisementCommandValidator : AbstractValidator<DeleteAdvertisementCommand>
{
    public DeleteAdvertisementCommandValidator()
    {
        RuleFor(x => x.AdvertisementId).ForeignKeyValidator();
    }
}

public class DeleteAdvertisementCommandHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<DeleteAdvertisementCommand, Result>
{
    public async Task<Result> Handle(DeleteAdvertisementCommand request, CancellationToken cancellationToken)
    {
        AdvertisementDto advertisement =
            await exhibitionService.GetAdvertisementById(request.AdvertisementId, cancellationToken);
        AuthorizationResult authResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(advertisement.ExhibitionId), Operations.Update);
        Guard.Against.UnAuthorized(authResult);

        Result result = await exhibitionService.DeleteAdvertisement(request.AdvertisementId, cancellationToken);
        return result;
    }
}
