#region

using RegisterMe.Application.Common.Extensions;
using RegisterMe.Application.ValueTypes;

#endregion

namespace RegisterMe.Application.Pricing.Dtos;

public class CatRegistrationPrice
{
    public required int CatRegistrationId { get; init; }
    public required MultiCurrencyPrice CatRegistrationPriceWithoutFees { get; init; }
    public required List<FeeRecord> CatRegistrationFees { get; init; }
    public required string CatName { get; init; }

    public MultiCurrencyPrice GetPrice()
    {
        return CatRegistrationPriceWithoutFees + CatRegistrationFees.Select(x => x.Price).Sum();
    }
}
