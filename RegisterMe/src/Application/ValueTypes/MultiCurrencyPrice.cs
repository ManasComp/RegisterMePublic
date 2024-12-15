#region

using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.ValueTypes;

public readonly struct MultiCurrencyPrice() : IEquatable<MultiCurrencyPrice>
{
    private const string Splitter = ",";

    public MultiCurrencyPrice(string encodedPrice) : this(
        int.Parse(encodedPrice.Split(Splitter)[0]),
        int.Parse(encodedPrice.Split(Splitter)[1])
    )
    {
        string[] prices = encodedPrice.Split(Splitter);
        if (prices.Length != 2)
        {
            throw new InvalidOperationException();
        }
    }

    public MultiCurrencyPrice(decimal priceCzk, decimal priceEur) : this()
    {
        PriceCzk = priceCzk;
        PriceEur = priceEur;
    }

    /// <summary>
    ///     Price in CZK
    /// </summary>
    public decimal PriceCzk { get; }

    /// <summary>
    ///     Price in EUR
    /// </summary>
    public decimal PriceEur { get; }

    public static MultiCurrencyPrice operator +(MultiCurrencyPrice a, MultiCurrencyPrice b)
    {
        return new MultiCurrencyPrice(a.PriceCzk + b.PriceCzk, a.PriceEur + b.PriceEur);
    }

    public bool Equals(MultiCurrencyPrice other)
    {
        return PriceCzk == other.PriceCzk && PriceEur == other.PriceEur;
    }

    public override bool Equals(object? obj)
    {
        return obj is MultiCurrencyPrice other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PriceCzk, PriceEur);
    }

    public static bool operator ==(MultiCurrencyPrice left, MultiCurrencyPrice right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(MultiCurrencyPrice left, MultiCurrencyPrice right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"{PriceCzk}{Splitter}{PriceEur}";
    }

    public decimal GetPriceForCurrency(Currency currency)
    {
        return currency switch
        {
            Currency.Czk => PriceCzk,
            Currency.Eur => PriceEur,
            _ => throw new ArgumentOutOfRangeException(nameof(currency), currency, null)
        };
    }

    public string GetStringForCurrency(Currency currency)
    {
        return GetPriceForCurrency(currency) + " " + currency.GetString();
    }
}
