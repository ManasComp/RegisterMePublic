#region

using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Organizations.Dtos;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class OrganizationModel
{
    public required PaginatedList<OrganizationDto> Organizations
    {
        get;
        init;
    } =
        null!;

    public required OrganizationFilterDto OrganizationsFilterDto { get; init; } = null!;
}
