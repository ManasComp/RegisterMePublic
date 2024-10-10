#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;

#endregion

namespace RegisterMe.Application.Cages.Queries.GetPersonCagesByExhibitionDay;

// ReSharper disable always UnusedType.Global
public record GetPersonCagesByExhibitionDayQuery : IRequest<List<CageDto>>
{
    public required int ExhibitionDayId { get; init; }
}

public class GetPersonCagesByExhibitionDayQueryValidator : AbstractValidator<GetPersonCagesByExhibitionDayQuery>
{
    public GetPersonCagesByExhibitionDayQueryValidator()
    {
        RuleFor(x => x.ExhibitionDayId).ForeignKeyValidator();
    }
}

public class GetPersonCagesByExhibitionDayQueryHandler(
    ICagesService cagesService,
    IAuthorizationService authorizationService,
    IUser user,
    IApplicationDbContext applicationDbContext
) : IRequestHandler<GetPersonCagesByExhibitionDayQuery, List<CageDto>>
{
    public async Task<List<CageDto>> Handle(GetPersonCagesByExhibitionDayQuery request,
        CancellationToken cancellationToken)
    {
        int exhibitionId =
            await applicationDbContext.ExhibitionDays
                .Where(x => x.Id == request.ExhibitionDayId)
                .Select(x => x.ExhibitionId)
                .SingleOrDefaultAsync(cancellationToken);
        AuthorizationResult authResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(exhibitionId), Operations.DoOrganizationAdminStuff);
        Guard.Against.UnAuthorized(authResult);

        List<CageDto> personCages =
            await cagesService.GetPersonCagesByExhibitionDay(request.ExhibitionDayId, null, cancellationToken);
        return personCages;
    }
}
