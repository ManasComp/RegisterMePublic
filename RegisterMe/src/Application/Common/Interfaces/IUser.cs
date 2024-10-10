#region

using System.Security.Claims;

#endregion

namespace RegisterMe.Application.Common.Interfaces;

public interface IUser
{
    string? Id { get; }
    ClaimsPrincipal? User { get; }
}
