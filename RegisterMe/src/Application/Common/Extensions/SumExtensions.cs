#region

using RegisterMe.Application.ValueTypes;

#endregion

namespace RegisterMe.Application.Common.Extensions;

public static class SumExtensions
{
    public static MultiCurrencyPrice Sum(
        this IEnumerable<MultiCurrencyPrice> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        IEnumerable<MultiCurrencyPrice> multiCurrencyPrices = source.ToList();
        return !multiCurrencyPrices.Any()
            ? new MultiCurrencyPrice(0, 0)
            : multiCurrencyPrices.Aggregate((x, y) => x + y);
    }
}
