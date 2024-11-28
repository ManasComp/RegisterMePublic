#region

using RegisterMe.Application.Organizations.Enums;

#endregion

namespace WebGui.Areas.Visitor.Models;

public record OrganizationFilterDto
{
    /// <summary>
    ///     Search by name, description and streetAddress
    /// </summary>
    public required string? SearchString { get; init; }

    public required OrganizationConfirmationStatus OrganizationConfirmationStatus { get; init; }
}
