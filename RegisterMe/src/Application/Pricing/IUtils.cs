#region

using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.Services.Enums;

#endregion

namespace RegisterMe.Application.Pricing;

/// <summary>
///     Interface for utility methods related to pricing.
/// </summary>
public interface IUtils
{
    /// <summary>
    ///     Determines if cages meet the specified criteria. Not inclusive.
    /// </summary>
    /// <param name="pricingCages">List of pricing cages to search through.</param>
    /// <param name="minLength">Minimum length of the cage.</param>
    /// <param name="height">Height of the cage.</param>
    /// <param name="width">Width of the cage.</param>
    /// <param name="cageType">Type of the cage.</param>
    /// <param name="numberOfDays">Number of days the cage is needed.</param>
    /// <returns>True if a cage meeting the criteria is found, otherwise false.</returns>
    bool FindAnyCage(List<PricingCage> pricingCages, int minLength, int height, int width,
        OwnCageEnum cageType, int numberOfDays);

    /// <summary>
    ///     Sets the price for a given cat registration structure.
    /// </summary>
    /// <param name="catRegistrationStructure">The cat registration structure to set the price for.</param>
    /// <param name="encodedPriceToSet">The encoded price to set.</param>
    /// <returns>The encoded discount as a string.</returns>
    string SetPrice(CatRegistrationStructure catRegistrationStructure,
        string encodedPriceToSet);

    /// <summary>
    ///     Creates new multi-currency price.
    /// </summary>
    /// <param name="priceCzk"></param>
    /// <param name="priceEur"></param>
    /// <returns></returns>
    string MultiCurrency(int priceCzk, int priceEur);
}
