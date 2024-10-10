namespace RegisterMe.Application.Cages.Dtos.Cage;

public record CageDto : CreateCageDto
{
    public required int NumberOfCatsInCage { get; set; }
}
