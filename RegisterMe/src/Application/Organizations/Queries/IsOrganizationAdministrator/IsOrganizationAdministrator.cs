#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;

#endregion

namespace RegisterMe.Application.Organizations.Queries.IsOrganizationAdministrator;

// ReSharper disable always UnusedType.Global
public record IsOrganizationAdministratorQuery : IRequest<bool>
{
    public required string UserId { get; init; }
    public required int OrganizationId { get; init; }
}

public class IsOrganizationAdministratorQueryValidator : AbstractValidator<IsOrganizationAdministratorQuery>
{
    public IsOrganizationAdministratorQueryValidator()
    {
        RuleFor(x => x.UserId).ForeignKeyValidator();
        RuleFor(x => x.OrganizationId).ForeignKeyValidator();
    }
}

public class IsOrganizationAdministratorQueryHandler(
    IOrganizationService organizationService,
    IUser user,
    IAuthorizationService authorizationService)
    : IRequestHandler<IsOrganizationAdministratorQuery, bool>
{
    public async Task<bool> Handle(IsOrganizationAdministratorQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult2 = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeOwnDataId(request.UserId),
                Operations.Read);
        Guard.Against.UnAuthorized(authorizationResult2);

        return await organizationService.IsOrganizationAdministrator(request.UserId, request.OrganizationId);
    }
}
