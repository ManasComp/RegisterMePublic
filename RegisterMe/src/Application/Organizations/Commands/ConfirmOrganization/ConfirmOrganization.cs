#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Organizations.Commands.ConfirmOrganization;

// ReSharper disable always UnusedType.Global
public record ConfirmOrganizationCommand : IRequest<Result>
{
    public required int OrganizationId { get; init; }
}

public class ConfirmOrganizationCommandValidator : AbstractValidator<ConfirmOrganizationCommand>
{
    public ConfirmOrganizationCommandValidator()
    {
        RuleFor(x => x.OrganizationId).ForeignKeyValidator();
    }
}

public class ConfirmOrganizationCommandHandler(
    IUser user,
    IAuthorizationService authorizationService,
    IOrganizationService organizationService)
    : IRequestHandler<ConfirmOrganizationCommand, Result>
{
    public async Task<Result> Handle(ConfirmOrganizationCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeOrganizationId(request.OrganizationId),
                Operations.DoSuperAdminStuff);
        Guard.Against.UnAuthorized(authorizationResult);

        Result result = await organizationService.ConfirmOrganization(request.OrganizationId, cancellationToken);
        return result;
    }
}
