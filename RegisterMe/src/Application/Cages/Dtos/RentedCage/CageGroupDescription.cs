namespace RegisterMe.Application.Cages.Dtos.RentedCage;

public record CageGroupDtoDescription
{
    public required RentedCageGroup RentedTypeId { get; init; }
    public required HashSet<int> Ids { get; init; } = [];
}
