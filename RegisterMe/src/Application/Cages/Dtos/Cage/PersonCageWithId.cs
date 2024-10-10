namespace RegisterMe.Application.Cages.Dtos.Cage;

public record PersonCageWithId
{
    public required CageDto PersonCage { get; init; }
    public required int PersonCageId { get; init; }
}
