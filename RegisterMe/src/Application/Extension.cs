#region

using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application;

internal static class EnumExtension
{
    public static String GetString(this Currency s1)
    {
        return s1 switch
        {
            Currency.Czk => "Kč",
            Currency.Eur => "\u20ac",
            _ => throw new ArgumentException()
        };
    }
}
