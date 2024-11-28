namespace WebApi.Endpoints.Request;

public sealed class RefreshReq
{
    /// <summary>
    ///     The last "/login" or "/refresh" response used to get a new
    ///     with an extended expiration.
    /// </summary>
    public required string RefreshToken { get; init; }
}
