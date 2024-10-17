#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetAdvertisementsByExhibitionId;

// ReSharper disable always UnusedType.Global
public record GetAdvertisementsByExhibitionIdQuery : IRequest<List<AdvertisementDto>>
{
    public required int ExhibitionId { get; init; }
}

public class GetAdvertisementsByExhibitionIdQueryValidator : AbstractValidator<GetAdvertisementsByExhibitionIdQuery>
{
    public GetAdvertisementsByExhibitionIdQueryValidator()
    {
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
    }
}

public class GetAdvertisementsByExhibitionIdQueryHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetAdvertisementsByExhibitionIdQuery, List<AdvertisementDto>>
{
    public async Task<List<AdvertisementDto>> Handle(GetAdvertisementsByExhibitionIdQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(request.ExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        return (await exhibitionService.GetAdvertisementsByExhibitionId(request.ExhibitionId, cancellationToken))
            .ToList();
    }
}
