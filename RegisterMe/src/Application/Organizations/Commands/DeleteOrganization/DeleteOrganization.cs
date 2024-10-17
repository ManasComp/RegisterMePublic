#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Organizations.Commands.DeleteOrganization;

// ReSharper disable always UnusedType.Global
public record DeleteOrganizationCommand : IRequest<Result>
{
    public required int OrganizationId { get; init; }
}

public class DeleteOrganizationCommandValidator : AbstractValidator<DeleteOrganizationCommand>
{
    public DeleteOrganizationCommandValidator()
    {
        RuleFor(x => x.OrganizationId).ForeignKeyValidator();
    }
}

public class DeleteOrganizationCommandHandler(
    IUser user,
    IAuthorizationService authorizationService,
    IOrganizationService organizationService)
    : IRequestHandler<DeleteOrganizationCommand, Result>
{
    public async Task<Result> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeOrganizationId(request.OrganizationId),
                Operations.Update);
        Guard.Against.UnAuthorized(authorizationResult);

        Result result = await organizationService.DeleteOrganization(request.OrganizationId, cancellationToken);
        return result;
    }
}
