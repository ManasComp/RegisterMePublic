#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitors.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitors.Queries.GetExhibitorByUserId;

// ReSharper disable always UnusedType.Global
public record GetExhibitorByUserIdQuery : IRequest<ExhibitorAndUserDto?>
{
    public required string UserId { get; init; } = null!;
}

public class GetExhibitorByUserIdQueryValidator : AbstractValidator<GetExhibitorByUserIdQuery>
{
    public GetExhibitorByUserIdQueryValidator()
    {
        RuleFor(x => x.UserId).ForeignKeyValidator();
    }
}

public class GetExhibitorByUserIdQueryHandler(
    IExhibitorService exhibitorService,
    IUser user,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetExhibitorByUserIdQuery, ExhibitorAndUserDto?>
{
    public async Task<ExhibitorAndUserDto?> Handle(GetExhibitorByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        ExhibitorAndUserDto? exhibitorAndUser =
            await exhibitorService.GetExhibitorByUserId(request.UserId, cancellationToken);

        if (exhibitorAndUser != null)
        {
            AuthorizationResult result = await authorizationService.AuthorizeAsync(
                AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitorId(exhibitorAndUser.Id), Operations.Read);
            Guard.Against.UnAuthorized(result);
        }
        else
        {
            AuthorizationResult result = await authorizationService.AuthorizeAsync(
                AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeOwnDataId(request.UserId), Operations.Read);
            Guard.Against.UnAuthorized(result);
        }

        return exhibitorAndUser;
    }
}
