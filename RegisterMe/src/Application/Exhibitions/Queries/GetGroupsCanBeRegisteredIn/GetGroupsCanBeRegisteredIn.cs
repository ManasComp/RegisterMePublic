#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Services.Groups;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetGroupsCanBeRegisteredIn;

// ReSharper disable always UnusedType.Global
public record GetGroupsCanBeRegisteredInQuery : IRequest<List<DatabaseGroupDto>>
{
    public required LitterOrExhibitedCatDto CatRegistration { get; init; } = null!;
    public required int ExhibitionDayId { get; init; }
}

public class GetGroupsCanBeRegisteredInQueryValidator : AbstractValidator<GetGroupsCanBeRegisteredInQuery>
{
    public GetGroupsCanBeRegisteredInQueryValidator()
    {
        RuleFor(x => x.CatRegistration).NotNull();
        RuleFor(x => x.ExhibitionDayId).NotNull();
        RuleFor(x => x.CatRegistration.ExhibitedCat).NotNull().When(x => x.CatRegistration.LitterDto == null);
        RuleFor(x => x.CatRegistration.LitterDto).NotNull().When(x => x.CatRegistration.ExhibitedCat == null);
    }
}

public class GetGroupsCanBeRegisteredInQueryHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user,
    IApplicationDbContext context)
    : IRequestHandler<GetGroupsCanBeRegisteredInQuery, List<DatabaseGroupDto>>
{
    public async Task<List<DatabaseGroupDto>> Handle(GetGroupsCanBeRegisteredInQuery request,
        CancellationToken cancellationToken)
    {
        int exhibitionId = context.ExhibitionDays
            .Where(x => x.Id == request.ExhibitionDayId)
            .Select(x => x.ExhibitionId)
            .FirstOrDefault();

        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(exhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        return await exhibitionService.GetGroupsCatRegistrationCanBeRegisteredIn(request.CatRegistration,
            request.ExhibitionDayId);
    }
}
