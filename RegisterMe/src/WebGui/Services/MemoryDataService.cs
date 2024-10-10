#region

using Microsoft.Extensions.Caching.Memory;
using RegisterMe.Application.CatRegistrations.Dtos;

#endregion

namespace WebGui.Services;

public class MemoryDataService(IMemoryCache memoryCache) : IMemoryDataService
{
    /// <inheritdoc />
    public void SetTemporaryCatRegistration(TemporaryCatRegistrationDto createCatRegistration,
        int registrationToExhibitionId)
    {
        memoryCache.Set(GetTemporaryCatRegistrationKey(registrationToExhibitionId), createCatRegistration,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1)));
    }

    /// <inheritdoc />
    public void DeleteTemporaryCatRegistration(int registrationToExhibitionId)
    {
        memoryCache.Remove(GetTemporaryCatRegistrationKey(registrationToExhibitionId));
    }

    /// <inheritdoc />
    public Task<TemporaryCatRegistrationDto?> GetTemporaryCatRegistration(int registrationToExhibitionId)
    {
        TemporaryCatRegistrationDto? catRegistration =
            memoryCache.Get<TemporaryCatRegistrationDto>(GetTemporaryCatRegistrationKey(registrationToExhibitionId));
        return Task.FromResult(catRegistration);
    }


    /// <inheritdoc />
    public void SetTemporaryPrice(TemporaryPriceDto createCatRegistration,
        int exhibitionId)
    {
        memoryCache.Set(GetTemporaryPriceKey(exhibitionId), createCatRegistration,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(0.5)));
    }

    /// <inheritdoc />
    public void DeleteTemporaryPrice(int exhibitionId)
    {
        memoryCache.Remove(GetTemporaryPriceKey(exhibitionId));
    }

    /// <inheritdoc />
    public Task<TemporaryPriceDto?> GetTemporaryPrice(int exhibitionId)
    {
        TemporaryPriceDto? catRegistration =
            memoryCache.Get<TemporaryPriceDto>(GetTemporaryPriceKey(exhibitionId));
        return Task.FromResult(catRegistration);
    }

    private static string GetTemporaryCatRegistrationKey(int registrationToExhibitionId)
    {
        return "TemporaryCatRegistration_" + registrationToExhibitionId;
    }

    private static string GetTemporaryPriceKey(int exhibitionId)
    {
        return "TemporaryPrice_" + exhibitionId;
    }
}
