#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;

#endregion

namespace RegisterMe.Application.Authorization;

/// <summary>
///     Organization authorization handler.
/// </summary>
/// <param name="serviceScopeFactory"></param>
/// <param name="authorizationHelperMethods"></param>
public class OrganizationsAuthorizationCrudHandler(
    IServiceScopeFactory serviceScopeFactory,
    AuthorizationHelperMethods authorizationHelperMethods) :
    Authorization<AuthorizeOrganizationId>(serviceScopeFactory, authorizationHelperMethods)
{
    protected override async Task HandleDeleteRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        AuthorizeOrganizationId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    protected override async Task HandleUpdateRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeOrganizationId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    protected override async Task HandleCreateRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeOrganizationId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    protected override async Task HandleReadRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeOrganizationId resource)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        bool isConfirmed = await applicationDbContext.Organizations
            .Where(x => x.Id == resource.OrganizationId)
            .Select(x => x.IsConfirmed)
            .SingleAsync();
        if (isConfirmed)
        {
            context.Succeed(requirement);
        }
        else
        {
            await DefaultRequirement(context, requirement, resource);
        }
    }

    private Task DefaultRequirement(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        AuthorizeOrganizationId resource)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        string userId = AuthorizationHelperMethods.GetUserIdPub(context);

        bool isOrganizationAdmin = applicationDbContext.Organizations
            .Where(x => x.Id == resource.OrganizationId)
            .Any(x => x.Administrator
                .Any(admin => admin.Id == userId));

        if (isOrganizationAdmin)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public record AuthorizeOrganizationId(int? OrganizationId);
