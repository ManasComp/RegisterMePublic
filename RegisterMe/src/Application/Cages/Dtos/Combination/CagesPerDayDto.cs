#region

using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.Cages.Dtos.RentedCage;

#endregion

namespace RegisterMe.Application.Cages.Dtos.Combination;

public record CagesPerDayDto
{
    public required List<CageGroupDtoDescription> AvailableCagesForRent { get; init; } = null!;
    public required List<CageGroupDtoDescription> AvailableAlreadyRentedCagesByExhibitor { get; init; } = null!;
    public required List<CageDtoDescription> ExhibitorsCages { get; init; } = null!;
}
