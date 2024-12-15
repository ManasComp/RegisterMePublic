namespace WebApi.Endpoints.Request;

public sealed class LoginReq
{
    /// <summary>
    ///     The user's email address which acts as a user name.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///     The user's password.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    ///     The optional two-factor authenticator code. This may be required for users who have enabled two-factor
    ///     authentication.
    ///     This is not required if a <see cref="TwoFactorRecoveryCode" /> is sent.
    /// </summary>
    public string? TwoFactorCode { get; init; }

    /// <summary>
    ///     An optional two-factor recovery code.
    ///     This is required for users who have enabled two-factor authentication but lost access to their
    ///     <see cref="TwoFactorCode" />.
    /// </summary>
    public string? TwoFactorRecoveryCode { get; init; }
}
