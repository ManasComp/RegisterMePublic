namespace WebApi.Endpoints.Request;

public sealed class InfoReq
{
    /// <summary>
    ///     The optional new email address for the authenticated user. This will replace the old email address if there was
    ///     one. The email will not be updated until it is confirmed.
    /// </summary>
    public string? NewEmail { get; init; }

    /// <summary>
    ///     The optional new password for the authenticated user. If a new password is provided, the <see cref="OldPassword" />
    ///     is required.
    ///     If the user forgot the old password, use the "/forgotPassword" endpoint instead.
    /// </summary>
    public string? NewPassword { get; init; }

    /// <summary>
    ///     The old password for the authenticated user. This is only required if a <see cref="NewPassword" /> is provided.
    /// </summary>
    public string? OldPassword { get; init; }

    /// <summary>
    ///     Phone number of the user.
    /// </summary>
    public string? PhoneNumber { get; init; }
}
