#region

using RegisterMe.Application.Services.Enums;
using RegisterMe.Application.ValueTypes;

#endregion

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace RegisterMe.Application.Pricing.Dtos;

public record CatRegistrationStructure
{
    /// <summary>
    ///     Cat registration id
    /// </summary>
    public required int CatRegistrationId { get; init; }

    /// <summary>
    ///     Index of the cat in the list of cats from RegistrationToExhibition sorted by price ascending
    /// </summary>
    public required int SortedAscendingByPriceIndex { get; set; }

    /// <summary>
    ///     Number of visited days
    /// </summary>
    public required int NumberOfVisitedDays { get; set; }

    /// <summary>
    ///     Original prices
    /// </summary>
    public required MultiCurrencyPrice OriginalPrice { get; init; }

    /// <summary>
    ///     Name of the registered cat
    /// </summary>
    public required string CatName { get; init; }

    /// <summary>
    ///     Ids of rented cage types
    /// </summary>
    public required List<int> RentedCageTypesIds { get; set; }

    /// <summary>
    ///     Dictionary of used cages per rented cage type
    /// </summary>
    public required Dictionary<CagesAndCatsEnum, int> CountOfUsedCagesPerRentedCageType { get; set; }

    /// <summary>
    ///     List of own cages
    /// </summary>
    public required List<PricingCage> OwnCages { get; set; }

    /// <summary>
    ///     Is the cat litter
    /// </summary>
    public required bool IsLitter { get; init; }
}
