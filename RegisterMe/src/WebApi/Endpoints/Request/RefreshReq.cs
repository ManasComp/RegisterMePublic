namespace WebApi.Endpoints.Request;

public sealed class RefreshReq
{
    /// <summary>
    ///     The <see cref="AccessTokenResponse.RefreshToken" /> from the last "/login" or "/refresh" response used to get a new
    ///     <see cref="AccessTokenResponse" />
    ///     with an extended expiration.
    /// </summary>
    public required string RefreshToken { get; init; }
}
