#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.Authorization.Helpers;

#endregion

namespace RegisterMe.Application.Authorization;

/// <summary>
///     Own data authorization handler.
/// </summary>
/// <param name="serviceScopeFactory"></param>
/// <param name="authorizationHelperMethods"></param>
public class OwnDataAuthorizationCrudHandler(
    IServiceScopeFactory serviceScopeFactory,
    AuthorizationHelperMethods authorizationHelperMethods) :
    Authorization<AuthorizeOwnDataId>(serviceScopeFactory, authorizationHelperMethods)
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        AuthorizeOwnDataId resource)
    {
        if (resource.RequestedDataForUserId == null)
        {
            context.Succeed(requirement);
        }

        await base.HandleRequirementAsync(context, requirement, resource);
    }

    private Task DefaultRequirement(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeOwnDataId resource)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();

        string userId = AuthorizationHelperMethods.GetUserIdPub(context);

        if (userId != resource.RequestedDataForUserId)
        {
            return Task.CompletedTask;
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }

    protected override async Task HandleDeleteRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeOwnDataId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    protected override async Task HandleUpdateRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeOwnDataId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    protected override async Task HandleCreateRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeOwnDataId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    protected override async Task HandleReadRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeOwnDataId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }
}

public record AuthorizeOwnDataId(string? RequestedDataForUserId);
