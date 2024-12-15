#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.RegistrationToExhibition;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Authorization;

/// <summary>
///     Exhibitor authorization handler.
/// </summary>
/// <param name="serviceScopeFactory"></param>
/// <param name="userManager"></param>
/// <param name="authorizationHelperMethods"></param>
public class ExhibitorAuthorizationCrudHandler(
    IServiceScopeFactory serviceScopeFactory,
    UserManager<ApplicationUser> userManager,
    AuthorizationHelperMethods authorizationHelperMethods) :
    Authorization<AuthorizeExhibitorId>(serviceScopeFactory, authorizationHelperMethods)
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        AuthorizeExhibitorId? resource)
    {
        if (resource != null)
        {
            if (requirement == Operations.OnlyOwnerCanDo)
            {
                await HandleOnlyOwnerCanDo(context, requirement, resource);
                return;
            }

            await base.HandleRequirementAsync(context, requirement, resource);
            return;
        }

        context.Succeed(requirement);
    }

    protected override async Task HandleDeleteRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeExhibitorId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    protected override async Task HandleUpdateRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeExhibitorId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    private async Task HandleOnlyOwnerCanDo(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeExhibitorId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    protected override async Task HandleCreateRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeExhibitorId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    protected override async Task HandleReadRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeExhibitorId resource)
    {
        List<int> organizations = await DefaultRequirement(context, requirement, resource);
        ApplicationUser? user = await userManager.FindByIdAsync(AuthorizationHelperMethods.GetUserIdPub(context));
        Guard.Against.Null(user, nameof(user));
        if (organizations.Any(x => x == user.OrganizationId))
        {
            context.Succeed(requirement);
        }
    }

    private async Task<List<int>> DefaultRequirement(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        AuthorizeExhibitorId resource)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        string userId = AuthorizationHelperMethods.GetUserIdPub(context);
        ApplicationUser? user = await userManager.FindByIdAsync(userId);
        Guard.Against.Null(user, nameof(user));
        Exhibitor exhibitor = await applicationDbContext.Exhibitors
            .Where(x => x.Id == resource.ExhibitorId)
            .Include(x => x.RegistrationToExhibitions)
            .ThenInclude(x => x.Exhibition)
            .Include(x => x.RegistrationToExhibitions)
            .ThenInclude(x => x.PaymentInfo)
            .SingleAsync();

        if (exhibitor.AspNetUserId == userId)
        {
            context.Succeed(requirement);
        }

        List<int> organizationsIsRegisteredTo = exhibitor.RegistrationToExhibitions.ToList()
            .Where(x => RegistrationToExhibitionService.IsPaid(x.PaymentInfo))
            .Select(x => x.Exhibition.OrganizationId).ToHashSet().ToList();

        return organizationsIsRegisteredTo;
    }
}

public record AuthorizeExhibitorId(int? ExhibitorId);
