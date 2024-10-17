#region

using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.Services.Enums;
using RegisterMe.Application.ValueTypes;

#endregion

namespace RegisterMe.Application.Pricing;

// ReSharper disable always UnusedType.Global
public class Utils : IUtils
{
    /// <inheritdoc />
    public bool FindAnyCage(List<PricingCage> pricingCages, int minLength, int height, int width,
        OwnCageEnum cageType, int numberOfDays)
    {
        int numberOfDaysFound = pricingCages
            .Where(pricingCage =>
                (pricingCage.Length > minLength || pricingCage.Height > height || pricingCage.Width > width) &&
                pricingCage.Type == cageType)
            .Sum(pricingCage => pricingCage.NumberOfDays);

        return numberOfDaysFound == numberOfDays;
    }

    /// <inheritdoc />
    public string MultiCurrency(int priceCzk, int priceEur)
    {
        return new MultiCurrencyPrice(priceCzk, priceEur).ToString();
    }

    /// <inheritdoc />
    public string SetPrice(CatRegistrationStructure catRegistrationStructure,
        string encodedPriceToSet)
    {
        MultiCurrencyPrice priceToSet = new(encodedPriceToSet);
        decimal currentPriceInCzk = catRegistrationStructure.OriginalPrice.PriceCzk;
        decimal currentPriceInEur = catRegistrationStructure.OriginalPrice.PriceEur;

        decimal czkDiscount = priceToSet.PriceCzk - currentPriceInCzk;
        decimal eurDiscount = priceToSet.PriceEur - currentPriceInEur;
        MultiCurrencyPrice discount = new(czkDiscount, eurDiscount);

        return discount.ToString();
    }
}
