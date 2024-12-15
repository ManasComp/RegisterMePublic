namespace WebApi.Endpoints.Request;

public sealed class RegisterReq
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
    ///     The user's first name.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    ///     The user's last name.
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    ///     The user's date of birth.
    /// </summary>
    public required DateOnly DateOfBirth { get; init; }

    /// <summary>
    ///     The user's phone number.
    /// </summary>
    public required string PhoneNumber { get; init; }
}
