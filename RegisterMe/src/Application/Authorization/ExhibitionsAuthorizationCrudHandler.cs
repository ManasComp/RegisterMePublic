#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;

#endregion

namespace RegisterMe.Application.Authorization;

/// <summary>
///     Exhibition authorization handler.
/// </summary>
/// <param name="serviceScopeFactory"></param>
/// <param name="authorizationHelperMethods"></param>
public class ExhibitionsAuthorizationCrudHandler(
    IServiceScopeFactory serviceScopeFactory,
    AuthorizationHelperMethods authorizationHelperMethods) :
    Authorization<AuthorizeExhibitionId>(serviceScopeFactory, authorizationHelperMethods)
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        AuthorizeExhibitionId resource)
    {
        if (requirement == Operations.DoOrganizationAdminStuff)
        {
            await HandleOrganizationAdminStuff(context, requirement, resource);
        }

        await base.HandleRequirementAsync(context, requirement, resource);
    }

    protected override async Task HandleDeleteRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeExhibitionId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    protected override async Task HandleUpdateRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeExhibitionId resource)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        bool isPublished = await applicationDbContext.Exhibitions
            .Where(x => x.Id == resource.ExhibitionId)
            .Select(x => x.IsPublished)
            .SingleOrDefaultAsync();
        bool isCancelled = await applicationDbContext.Exhibitions
            .Where(x => x.Id == resource.ExhibitionId)
            .Select(x => x.IsCancelled)
            .SingleOrDefaultAsync();
        if (!isPublished && !isCancelled)
        {
            await DefaultRequirement(context, requirement, resource);
        }
    }

    protected override async Task HandleCreateRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeExhibitionId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    protected override async Task HandleReadRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeExhibitionId resource)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        bool isPublished = await applicationDbContext.Exhibitions
            .Where(x => x.Id == resource.ExhibitionId)
            .Select(x => x.IsPublished)
            .SingleOrDefaultAsync();
        if (isPublished)
        {
            context.Succeed(requirement);
        }
        else
        {
            await DefaultRequirement(context, requirement, resource);
        }
    }

    private Task DefaultRequirement(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        AuthorizeExhibitionId resource)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        string userId = AuthorizationHelperMethods.GetUserIdPub(context);

        bool isOrganizationAdmin = applicationDbContext.Exhibitions
            .Where(x => x.Id == resource.ExhibitionId)
            .Any(x => x.Organization.Administrator
                .Any(admin => admin.Id == userId));

        if (isOrganizationAdmin)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private Task HandleOrganizationAdminStuff(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeExhibitionId resource)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        string userId = AuthorizationHelperMethods.GetUserIdPub(context);
        bool isOrganizationAdmin = applicationDbContext.Exhibitions
            .Any(x => x.Organization.Administrator
                .Any(admin => admin.Id == userId) && x.Id == resource.ExhibitionId);
        if (isOrganizationAdmin)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public record AuthorizeExhibitionId(int? ExhibitionId);
