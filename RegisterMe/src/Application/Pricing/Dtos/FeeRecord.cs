#region

using RegisterMe.Application.ValueTypes;

#endregion

namespace RegisterMe.Application.Pricing.Dtos;

public record FeeRecord
{
    public required string FeeName { get; init; }
    public required MultiCurrencyPrice Price { get; init; }
}
