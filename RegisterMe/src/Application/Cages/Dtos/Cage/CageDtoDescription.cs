namespace RegisterMe.Application.Cages.Dtos.Cage;

public record CageDtoDescription
{
    public required int CageId { get; init; }
    public required string Description { get; init; } = null!;
}
