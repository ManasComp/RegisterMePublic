#region

using RegisterMe.Application.ValueTypes;

#endregion

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace RegisterMe.Application.Pricing.Dtos;

public class RegistrationToExhibitionPrice
{
    public required int RegistrationToExhibitionId { get; set; }
    public required MultiCurrencyPrice AdvertisementPrice { get; init; }
    public required List<CatRegistrationPrice> CatRegistrationPrices { get; init; }
    public MultiCurrencyPrice GExhibitorPrice { get; set; }
    public MultiCurrencyPrice GTotalPrice { get; set; }
}
