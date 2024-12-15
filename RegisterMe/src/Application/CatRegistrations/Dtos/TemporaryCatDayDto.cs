#region

using RegisterMe.Application.Cages;
using RegisterMe.Application.Cages.Dtos.Cage;

#endregion

namespace RegisterMe.Application.CatRegistrations.Dtos;

public class TemporaryCatDayDto
{
    public required RentedCageGroup? RentedCageTypeId { get; init; }
    public required PersonCageDto? Cage { get; init; }
    public required int ExhibitionDayId { get; init; }
    public required List<string> GroupsIds { get; init; } = [];
}
