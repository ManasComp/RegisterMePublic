#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Validators;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.UpdateAdvertisement;

// ReSharper disable always UnusedType.Global
public record UpdateAdvertisementCommand : IRequest<Result>
{
    public required UpsertAdvertisementDto AdvertisementDto { get; init; } = null!;
    public required int AdvertisementId { get; init; }
}

public class
    UpdateAdvertisementCommandValidator : BaseAdvertisement.BaseAdvertisementCommandValidator<
    UpdateAdvertisementCommand>
{
    public UpdateAdvertisementCommandValidator()
    {
        AddCommonRules(x => x.AdvertisementDto);
        RuleFor(x => x.AdvertisementId).ForeignKeyValidator();
    }
}

public class UpdateAdvertisementCommandHandler(
    IExhibitionService exhibitionService,
    IUser user,
    IAuthorizationService authorizationService)
    : IRequestHandler<UpdateAdvertisementCommand, Result>
{
    public async Task<Result> Handle(UpdateAdvertisementCommand request, CancellationToken cancellationToken)
    {
        AdvertisementDto advertisement =
            await exhibitionService.GetAdvertisementById(request.AdvertisementId, cancellationToken);
        AuthorizationResult authResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(advertisement.ExhibitionId), Operations.Update);
        Guard.Against.UnAuthorized(authResult);
        Result result = await exhibitionService.UpdateAdvertisement(request.AdvertisementDto, request.AdvertisementId,
            cancellationToken);
        return result;
    }
}
