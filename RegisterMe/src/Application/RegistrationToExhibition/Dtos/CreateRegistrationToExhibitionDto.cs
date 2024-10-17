namespace RegisterMe.Application.RegistrationToExhibition.Dtos;

public record CreateRegistrationToExhibitionDto
{
    /// <summary>
    ///     Exhibition id
    /// </summary>
    public required int ExhibitionId { get; init; }

    /// <summary>
    ///     Exhibitor id
    /// </summary>
    public required int ExhibitorId { get; init; }

    /// <summary>
    ///     Advertisement id
    /// </summary>
    public required int AdvertisementId { get; init; }
}
