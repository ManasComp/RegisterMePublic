#region

using RegisterMe.Application.Organizations.Enums;

#endregion

namespace RegisterMe.Application.Organizations.Dtos;

public record GetOrganizationsModel
{
    public required string SearchString { get; init; } = "";
    public required HasExhibitions? HasExhibitions { get; init; }
}
