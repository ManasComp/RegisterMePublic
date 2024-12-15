#region

using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.CatRegistrations;

public interface ICatRegistrationService
{
    /// <summary>
    ///     Creates a new CatRegistration
    /// </summary>
    /// <param name="createCatRegistrationDto"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="isExhibitionOrganizer"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    Task<Result<int>> CreateCatRegistration(CreateCatRegistrationDto createCatRegistrationDto,
        bool isExhibitionOrganizer,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a CatRegistration with all its related entities
    /// </summary>
    /// <param name="catRegistrationId"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="isExhibitionOrganizer"></param>
    /// <returns></returns>
    Task<Result> DeleteCatRegistration(int catRegistrationId, bool isExhibitionOrganizer,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates catRegistration
    /// </summary>
    /// <param name="updateCatRegistrationDto"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="isExhibitionOrganizer"></param>
    /// <returns></returns>
    Task<Result<int>> UpdateCatRegistration(UpdateCatRegistrationDto updateCatRegistrationDto,
        bool isExhibitionOrganizer,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets CatRegistration by id
    /// </summary>
    /// <param name="catRegistrationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CatRegistrationDto> GetCatRegistrationById(int catRegistrationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets CatRegistrations by RegistrationToExhibitionId
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<MiddleCatRegistrationDto>> GetCatRegistrationsByRegistrationToExhibitionId(
        int registrationToExhibitionId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets CatRegistration price
    /// </summary>
    /// <param name="catRegistrationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<MultiCurrencyPrice> GetCatRegistrationPrice(int catRegistrationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets last litter
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <returns></returns>
    public Task<List<CatModelP>> GetUserLittersNotInExhibition(int registrationToExhibitionId);

    /// <summary>
    ///     Gets last ExhibitedCat
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <returns></returns>
    public Task<List<CatModelP>> GetUserCatsNotInExhibition(int registrationToExhibitionId);
}
