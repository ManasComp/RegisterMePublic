#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;

#endregion

namespace RegisterMe.Application.Cages.Queries.GetRentedCageGroupById;

// ReSharper disable always UnusedType.Global
public record GetRentedCageGroupByIdQuery : IRequest<BriefCageDto>
{
    public required string CagesId { get; init; }
}

public class GetRentedCageGroupByIdQueryValidator : AbstractValidator<GetRentedCageGroupByIdQuery>
{
    public GetRentedCageGroupByIdQueryValidator()
    {
        RuleFor(x => x.CagesId).NotEmpty();
    }
}

public class GetRentedCageGroupByIdQueryHandler(
    ICagesService cagesService,
    IApplicationDbContext applicationDbContext,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetRentedCageGroupByIdQuery, BriefCageDto>
{
    public async Task<BriefCageDto> Handle(GetRentedCageGroupByIdQuery request,
        CancellationToken cancellationToken)
    {
        List<int> ids = CagesService.FromGroupIdToIds(request.CagesId);
        if (ids.Count != 0)
        {
            int exhibitionId = await applicationDbContext.RentedCages
                .Where(x => x.Id == ids[0])
                .SelectMany(x => x.ExhibitionDays)
                .Select(x => x.ExhibitionId)
                .FirstOrDefaultAsync(cancellationToken);
            if (exhibitionId == 0)
            {
                throw new NotFoundException(nameof(exhibitionId), request.CagesId);
            }

            AuthorizationResult authResult = await authorizationService.AuthorizeAsync(
                AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(exhibitionId), Operations.Read);
            Guard.Against.UnAuthorized(authResult);
        }

        return await cagesService.GetRentedCageByGroupId(request.CagesId);
    }
}
