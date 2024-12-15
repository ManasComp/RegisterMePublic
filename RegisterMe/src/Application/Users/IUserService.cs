#region

using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Users;

public interface IUserService
{
    /// <summary>
    ///     Delete personal data
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> DeletePersonalDataAsync(string userId, CancellationToken cancellationToken = default);
}
