#region

using RegisterMe.Application.CatRegistrations.Dtos;

#endregion

namespace WebGui.Services;

public interface IMemoryDataService
{
    /// <summary>
    ///     Sets temporary cat registration
    /// </summary>
    /// <param name="createCatRegistration"></param>
    /// <param name="registrationToExhibitionId"></param>
    void SetTemporaryCatRegistration(TemporaryCatRegistrationDto createCatRegistration,
        int registrationToExhibitionId);

    /// <summary>
    ///     Deletes temporary cat registration
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    void DeleteTemporaryCatRegistration(int registrationToExhibitionId);

    /// <summary>
    ///     Gets temporary cat registration
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <returns></returns>
    Task<TemporaryCatRegistrationDto?> GetTemporaryCatRegistration(int registrationToExhibitionId);

    /// <summary>
    ///     Sets temporary price
    /// </summary>
    /// <param name="createCatRegistration"></param>
    /// <param name="registrationToExhibitionId"></param>
    public void SetTemporaryPrice(TemporaryPriceDto createCatRegistration,
        int registrationToExhibitionId);

    /// <summary>
    ///     Deletes temporary price
    /// </summary>
    /// <param name="exhibitionId"></param>
    public void DeleteTemporaryPrice(int exhibitionId);

    /// <summary>
    ///     Temporary price
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <returns></returns>
    public Task<TemporaryPriceDto?> GetTemporaryPrice(int exhibitionId);
}
