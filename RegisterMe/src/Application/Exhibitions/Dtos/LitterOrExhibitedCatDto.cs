#region

using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Exhibitors.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record LitterOrExhibitedCatDto
{
    public required CreateExhibitedCatDto? ExhibitedCat { get; init; }
    public required CreateLitterDto? LitterDto { get; init; }
    public required UpsertExhibitorDto ExhibitorDto { get; init; }
}
