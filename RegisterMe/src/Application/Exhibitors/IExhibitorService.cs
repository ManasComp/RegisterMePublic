#region

using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitors;

public interface IExhibitorService
{
    /// <summary>
    ///     Create exhibitor
    /// </summary>
    /// <param name="exhibitorDto"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<int>> CreateExhibitor(UpsertExhibitorDto exhibitorDto, string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get exhibitor by userId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExhibitorAndUserDto?> GetExhibitorByUserId(string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get exhibitor by id
    /// </summary>
    /// <param name="exhibitorId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExhibitorAndUserDto> GetExhibitorById(int exhibitorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get exhibitor by registrationToExhibitionId
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ExhibitorAndUserDto> GetExhibitorByRegistrationToExhibitionId(int registrationToExhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update exhibitor
    /// </summary>
    /// <param name="exhibitorDto"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> UpdateExhibitor(UpsertExhibitorDto exhibitorDto, string userId,
        CancellationToken cancellationToken = default);
}
