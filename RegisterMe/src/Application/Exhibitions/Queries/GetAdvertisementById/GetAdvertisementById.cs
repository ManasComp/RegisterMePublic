#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetAdvertisementById;

// ReSharper disable always UnusedType.Global
public record GetAdvertisementByIdQuery : IRequest<AdvertisementDto>
{
    public required int AdvertisementId { get; init; }
}

public class GetAdvertisementByIdQueryValidator : AbstractValidator<GetAdvertisementByIdQuery>
{
    public GetAdvertisementByIdQueryValidator()
    {
        RuleFor(x => x.AdvertisementId).ForeignKeyValidator();
    }
}

public class GetAdvertisementByIdQueryHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetAdvertisementByIdQuery, AdvertisementDto>
{
    public async Task<AdvertisementDto> Handle(GetAdvertisementByIdQuery request, CancellationToken cancellationToken)
    {
        AdvertisementDto advertisement =
            await exhibitionService.GetAdvertisementById(request.AdvertisementId, cancellationToken);

        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(advertisement.ExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        return advertisement;
    }
}
