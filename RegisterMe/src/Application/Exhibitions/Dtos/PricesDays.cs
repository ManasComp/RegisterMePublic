#region

using System.Diagnostics.CodeAnalysis;
using RegisterMe.Application.ValueTypes;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record PriceDays
{
    public PriceDays()
    {
    }

    [SetsRequiredMembers]
    public PriceDays(List<int> exhibitionDayIds, MultiCurrencyPrice price)
    {
        ExhibitionDayIds = exhibitionDayIds;
        Price = price;
    }

    public required List<int> ExhibitionDayIds { get; init; } = [];
    public required MultiCurrencyPrice Price { get; init; }
}
