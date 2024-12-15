#region

using Microsoft.Extensions.Configuration;

#endregion

namespace RegisterMe.Application.System;

public interface ISystemService
{
    /// <summary>
    ///     Tells if the payment by card is configured.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    bool PaymentByCardIsConfigured(IConfiguration config);
}
