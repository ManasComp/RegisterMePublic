#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Cages;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Services.Groups;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetGroupsByPriceGroupId;

// ReSharper disable always UnusedType.Global
public record GetGroupsByPriceGroupIdQuery : IRequest<List<DatabaseGroupDto>>
{
    public required string GroupId { get; init; }
}

public class GetGroupsByPriceGroupIdQueryValidator : AbstractValidator<GetGroupsByPriceGroupIdQuery>
{
    public GetGroupsByPriceGroupIdQueryValidator()
    {
        RuleFor(x => x.GroupId).NotNull();
    }
}

public class GetGroupsByPriceGroupIdQueryHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user,
    IApplicationDbContext context)
    : IRequestHandler<GetGroupsByPriceGroupIdQuery, List<DatabaseGroupDto>>
{
    public async Task<List<DatabaseGroupDto>> Handle(GetGroupsByPriceGroupIdQuery request,
        CancellationToken cancellationToken)
    {
        HashSet<int> pricesId = CagesService.FromGroupIdToIds(request.GroupId).ToHashSet();
        int exhibitionId = context.Prices
            .Include(x => x.ExhibitionDays)
            .Where(x => pricesId.Contains(x.Id))
            .SelectMany(x => x.ExhibitionDays)
            .Select(y => y.ExhibitionId)
            .FirstOrDefault();

        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(exhibitionId), Operations.DoOrganizationAdminStuff);
        Guard.Against.UnAuthorized(result);

        return await exhibitionService.GetGroupsByGroupId(request.GroupId);
    }
}
