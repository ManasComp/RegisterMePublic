#region

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Authorization.Helpers;

public class AuthorizationHelperMethods(IServiceScopeFactory serviceScopeFactory)
{
    private static string GetUserId(AuthorizationHandlerContext context)
    {
        ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)context.User.Identity;
        string? userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guard.Against.NullOrEmpty(userId, nameof(userId));

        return userId;
    }

    private static bool IsInRole(AuthorizationHandlerContext authorizationHandlerContext, string role)
    {
        ClaimsPrincipal claimsIdentity = authorizationHandlerContext.User;
        return claimsIdentity.IsInRole(role);
    }

    private async Task<bool> IsInRole(string? userId, string role)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        UserManager<ApplicationUser> userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = userManager.Users.SingleOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return false;
        }

        bool isInRole = await userManager.IsInRoleAsync(user, role);
        return isInRole;
    }

    public async Task<bool> IsInRole(AuthorizationHandlerContext? authorizationHandlerContext, string? userId,
        string role)
    {
        if (authorizationHandlerContext?.User.Claims.Any() == true)
        {
            return IsInRole(authorizationHandlerContext, role);
        }

        return await IsInRole(userId, role);
    }

    public string GetUserIdPub(AuthorizationHandlerContext? context)
    {
        if (context != null)
        {
            try
            {
                return GetUserId(context);
            }
            catch (ArgumentException)
            {
                // it is badly mocked, so catch it for tests
            }
        }

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IUser user = scope.ServiceProvider.GetRequiredService<IUser>();
        string? id = user.Id;
        Guard.Against.NullOrEmpty(id);
        return id;
    }

    public static ClaimsPrincipal ThrowExceptionIfUserNotLoggedIn(IUser? user)
    {
        if (user == null)
        {
            throw new ForbiddenAccessException();
        }

        ClaimsPrincipal? userClaim = user.User;
        if (userClaim == null || user.Id == null)
        {
            throw new ForbiddenAccessException();
        }

        return userClaim;
    }

    public static int GetRegistrationToExhibitionIdFromCatRegistrationId(IApplicationDbContext applicationDbContext,
        int catRegistrationId)
    {
        return applicationDbContext.CatRegistrations
            .Where(x => x.Id == catRegistrationId)
            .Select(x => x.RegistrationToExhibitionId)
            .FirstOrDefault();
    }
}
