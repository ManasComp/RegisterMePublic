#region

using Microsoft.AspNetCore.Mvc.Rendering;
using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class ExhibitionModel
{
    public required PaginatedList<ExhibitionDto> Exhibitions { get; init; } = null!;
    public required ExhibitionsFilterDto ExhibitionsFilterDto { get; init; } = null!;
    public required List<SelectListItem> Organizations { get; init; } = null!;
    public required int? OrganizationId { get; init; }
}
