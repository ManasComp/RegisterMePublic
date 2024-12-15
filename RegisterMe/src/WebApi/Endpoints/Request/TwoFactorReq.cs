namespace WebApi.Endpoints.Request;

public sealed class TwoFactorReq
{
    public bool? Enable { get; init; }

    public string? TwoFactorCode { get; init; }

    public bool ResetSharedKey { get; init; }

    public bool ResetRecoveryCodes { get; init; }

    public bool ForgetMachine { get; init; }
}
