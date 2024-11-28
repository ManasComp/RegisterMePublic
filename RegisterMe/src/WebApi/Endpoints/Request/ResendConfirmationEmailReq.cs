namespace WebApi.Endpoints.Request;

public sealed class ResendConfirmationEmailReq
{
    /// <summary>
    ///     The email address to resend the confirmation email to if a user with that email exists.
    /// </summary>
    public required string Email { get; init; }
}
