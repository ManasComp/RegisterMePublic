#region

using RegisterMe.Application.Exhibitions.Enums;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record ExhibitionsFilterDto
{
    /// <summary>
    ///     Filters exhibition only owned by the organization
    /// </summary>
    public required int? OrganizationId { get; init; }

    /// <summary>
    ///     Filters exhibition only where the user is registered to
    /// </summary>
    public required string? UserId { get; init; }

    /// <summary>
    ///     Search by name, description and streetAddress
    /// </summary>
    public required string? SearchString { get; init; }

    /// <summary>
    ///     Time frame fot exhibitions
    /// </summary>
    public required ExhibitionRegistrationStatus? ExhibitionRegistrationStatus { get; init; }

    /// <summary>
    ///     Users can filter by published, not published and all can do only admins
    /// </summary>
    public required OrganizationPublishStatus OrganizationPublishStatus { get; init; }
}
