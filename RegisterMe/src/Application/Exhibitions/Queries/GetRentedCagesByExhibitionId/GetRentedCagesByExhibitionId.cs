#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Cages;
using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetRentedCagesByExhibitionId;

// ReSharper disable always UnusedType.Global
public record GetRentedCagesByExhibitionIdQuery : IRequest<List<BriefCageDto>>
{
    public required int ExhibitionId { get; init; }
}

public class GetRentedCagesByExhibitionIdQueryValidator : AbstractValidator<GetRentedCagesByExhibitionIdQuery>
{
    public GetRentedCagesByExhibitionIdQueryValidator()
    {
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
    }
}

public class GetRentedCagesByExhibitionIdQueryHandler(
    ICagesService service,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetRentedCagesByExhibitionIdQuery, List<BriefCageDto>>
{
    public async Task<List<BriefCageDto>> Handle(GetRentedCagesByExhibitionIdQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(request.ExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        return await service.GetRentedCagesByExhibitionId(request.ExhibitionId);
    }
}
