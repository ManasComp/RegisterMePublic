namespace WebApi.Endpoints.Responses;

public sealed class TwoFactorResponse
{
    /// <summary>
    ///     The shared key generally for TOTP authenticator apps that is usually presented to the user as a QR code.
    /// </summary>
    public required string SharedKey { get; init; }

    /// <summary>
    ///     The number of unused <see cref="RecoveryCodes" /> remaining.
    /// </summary>
    public required int RecoveryCodesLeft { get; init; }

    /// <summary>
    ///     The recovery codes to use if the <see cref="SharedKey" /> is lost. This will be omitted from the response unless
    /// </summary>
    public string[]? RecoveryCodes { get; init; }

    /// <summary>
    ///     Whether or not two-factor login is required for the current authenticated user.
    /// </summary>
    public required bool IsTwoFactorEnabled { get; init; }

    /// <summary>
    ///     Whether or not the current client has been remembered by two-factor authentication cookies. This is always
    ///     <see langword="false" /> for non-cookie authentication schemes.
    /// </summary>
    public required bool IsMachineRemembered { get; init; }
}
