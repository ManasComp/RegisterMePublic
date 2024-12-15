#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetDaysByGroupId;

// ReSharper disable always UnusedType.Global
public record GetDaysByGroupIdQuery : IRequest<List<ExhibitionDayDto>>
{
    public required string PriceGroupIds { get; init; }
}

public class GetDaysByGroupIdQueryValidator : AbstractValidator<GetDaysByGroupIdQuery>
{
    public GetDaysByGroupIdQueryValidator()
    {
        RuleFor(x => x.PriceGroupIds).NotNull();
    }
}

public class GetDaysByGroupIdQueryHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user,
    IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetDaysByGroupIdQuery, List<ExhibitionDayDto>>
{
    public async Task<List<ExhibitionDayDto>> Handle(GetDaysByGroupIdQuery request, CancellationToken cancellationToken)
    {
        List<ExhibitionDayDto> days =
            await exhibitionService.GetDaysByGroupId(request.PriceGroupIds, cancellationToken);
        if (days.Count == 0)
        {
            throw new NotFoundException("Group has no days", request.PriceGroupIds);
        }

        int exhibitionId = applicationDbContext.ExhibitionDays
            .Where(x => x.Id == days.First().Id)
            .Select(x => x.ExhibitionId)
            .FirstOrDefault();
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(exhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        return days;
    }
}
