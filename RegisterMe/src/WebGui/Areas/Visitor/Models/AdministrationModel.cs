#region

using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Organizations.Dtos;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class AdministratorModel
{
    public required PaginatedList<ExhibitionDto> Exhibitions { get; init; } = null!;

    public required OrganizationDto OrganizationDto { get; init; } = null!;
}
