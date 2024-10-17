namespace WebApi.Endpoints.Responses;

public sealed class InfoResponse
{
    /// <summary>
    ///     The email address associated with the authenticated user.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///     Indicates whether or not the <see cref="Email" /> has been confirmed yet.
    /// </summary>
    public required bool IsEmailConfirmed { get; init; }

    /// <summary>
    ///     The phone number associated with the authenticated user.
    /// </summary>
    public required string PhoneNumber { get; init; }

    public required string Id { get; init; }
}
