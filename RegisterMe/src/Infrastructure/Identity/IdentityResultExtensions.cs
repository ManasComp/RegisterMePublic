#region

using IdentityResult = RegisterMe.Application.Common.Models.IdentityResult;

#endregion

namespace RegisterMe.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static IdentityResult ToApplicationResult(this Microsoft.AspNetCore.Identity.IdentityResult result)
    {
        return result.Succeeded
            ? IdentityResult.Success()
            : IdentityResult.Failure(result.Errors.Select(e => e.Description));
    }
}
