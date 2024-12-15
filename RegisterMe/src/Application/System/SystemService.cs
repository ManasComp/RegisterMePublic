#region

using Microsoft.Extensions.Configuration;

#endregion

namespace RegisterMe.Application.System;

public class SystemService : ISystemService
{
    public bool PaymentByCardIsConfigured(IConfiguration config)
    {
        const string stripeKey = "Stripe:SecretKey";
        string? configKey = config.GetSection(stripeKey).Value;
        return !string.IsNullOrEmpty(configKey);
    }
}
