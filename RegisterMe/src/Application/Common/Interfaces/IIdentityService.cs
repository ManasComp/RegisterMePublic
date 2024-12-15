#region

using RegisterMe.Application.Common.Models;

#endregion

namespace RegisterMe.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<(IdentityResult Result, string UserId)> CreateUserAsync(string userName, string password);
    Task<IdentityResult> DeleteUserAsync(string userId);
}
