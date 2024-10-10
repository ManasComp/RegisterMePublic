#region

using System.Security.Claims;
using RegisterMe.Application.Common.Interfaces;

#endregion

namespace WebGui.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    public string? Id => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    public ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;
}
