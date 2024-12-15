#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Domain.Constants;

#endregion

namespace RegisterMe.Application.Authorization;

public abstract class Authorization<T2>(
    IServiceScopeFactory serviceScopeFactory,
    AuthorizationHelperMethods authorizationHelperMethods)
    : AuthorizationHandler<OperationAuthorizationRequirement, T2>
{
    protected readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    protected AuthorizationHelperMethods AuthorizationHelperMethods => authorizationHelperMethods;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        T2 resource)
    {
        if (context.HasSucceeded)
        {
            return;
        }

        if (await AuthorizationHelperMethods.IsInRole(context, authorizationHelperMethods.GetUserIdPub(context),
                Roles.Administrator))
        {
            context.Succeed(requirement);
            return;
        }

        Task authorizationHandler = requirement switch
        {
            not null when requirement.Name == Operations.Read.Name => HandleReadRequirementAsync(context, requirement,
                resource),
            not null when requirement.Name == Operations.Create.Name => HandleCreateRequirementAsync(context,
                requirement, resource),
            not null when requirement.Name == Operations.Update.Name => HandleUpdateRequirementAsync(context,
                requirement, resource),
            not null when requirement.Name == Operations.Delete.Name => HandleDeleteRequirementAsync(context,
                requirement, resource),
            _ => Task.CompletedTask
        };

        await authorizationHandler;
    }

    protected abstract Task HandleDeleteRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, T2 resource);

    protected abstract Task HandleUpdateRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, T2 resource);

    protected abstract Task HandleCreateRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, T2 resource);

    protected abstract Task HandleReadRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, T2 resource);
}
