#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Cages.Dtos.Cage;

public record PersonCageWithCatDays
{
    public required PersonCageWithId PersonCageWithId { get; init; }
    public required List<CatDay> CatDays { get; init; }
}
