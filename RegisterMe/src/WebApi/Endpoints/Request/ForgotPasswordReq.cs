namespace WebApi.Endpoints.Request;

public sealed class ForgotPasswordReq
{
    /// <summary>
    ///     The email address to send the reset password code to if a user with that confirmed email address already exists.
    /// </summary>
    public required string Email { get; init; }
}
