#region

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Domain.Entities;
using IdentityResult = RegisterMe.Application.Common.Models.IdentityResult;

#endregion

namespace RegisterMe.Infrastructure.Identity;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService)
    : IIdentityService
{
    public async Task<string?> GetUserNameAsync(string userId)
    {
        ApplicationUser user = await userManager.Users.FirstAsync(u => u.Id == userId);

        return user.UserName;
    }

    public async Task<(IdentityResult Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        ApplicationUser user = new() { UserName = userName, Email = userName };

        Microsoft.AspNetCore.Identity.IdentityResult result = await userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        ApplicationUser? user = userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        ApplicationUser? user = userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        ClaimsPrincipal principal = await userClaimsPrincipalFactory.CreateAsync(user);

        AuthorizationResult result = await authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<IdentityResult> DeleteUserAsync(string userId)
    {
        ApplicationUser? user = userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null ? await DeleteUserAsync(user) : IdentityResult.Success();
    }

    public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
    {
        Microsoft.AspNetCore.Identity.IdentityResult result = await userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }
}
