namespace RegisterMe.Infrastructure;

public static class GetWebAddress
{
    private static string? s_webAddress;

    public static string WebAddress
    {
        get
        {
            if (s_webAddress == null)
            {
                throw new Exception("WebAddress is not set.");
            }

            return s_webAddress;
        }
        set
        {
            if (s_webAddress != null)
            {
                throw new Exception("WebAddress is already set.");
            }

            s_webAddress = value;
        }
    }

    public static bool WasInitialized => s_webAddress != null;
}
