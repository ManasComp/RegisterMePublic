#region

using System.Globalization;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Services.Converters;

public class StringInvoiceFormatter
{
    public string Format(bool value)
    {
        return value ? "☑" : "☐";
    }

    public string Format(DateTimeOffset value)
    {
        return value.ToString("dd.MM.yyyy HH:mm:ss");
    }

    public string FormatWithYear(DateOnly? value)
    {
        return value == null ? "" : value.Value.ToString("dd.MM.yyyy");
    }

    public string Format(PaymentType value)
    {
        string formattedValue = value switch
        {
            PaymentType.PayByBankTransfer => "Převodem na účet",
            PaymentType.PayOnlineByCard => "Kartou online",
            PaymentType.PayInPlaceByCache => "Na místě hotovostí",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
        return formattedValue;
    }

    public string Format(PaymentType? value)
    {
        return value == null ? "" : Format(value.Value);
    }

    public string FormatWithDay(DateOnly value)
    {
        CultureInfo czechCulture = new("cs-CZ");
        return value.ToString("dddd dd.MM", czechCulture);
    }

    public string Format(string? value)
    {
        return value ?? "";
    }

    public string Format(decimal? value)
    {
        return value == null ? "" : Format(value.Value);
    }

    public string Format(int? value)
    {
        return value == null ? "" : value.Value.ToString();
    }

    public string Format(DateTimeOffset? value)
    {
        return value == null ? "" : Format(value.Value);
    }

    public string Format(bool? value)
    {
        return value == null ? "N/A" : Format(value.Value);
    }

    public string Format(decimal value)
    {
        return value.ToString("0");
    }

    public string Format(decimal value, Currency currency)
    {
        return Format(value) + " " + currency;
    }

    public string Format(decimal? value, Currency? currency)
    {
        if (value == null || currency == null)
        {
            return "";
        }

        return Format(value.Value, currency.Value);
    }

    public string Format(List<DateOnly> dates)
    {
        dates.Sort();
        return string.Join(", ", dates.Select(FormatWithDay));
    }
}
