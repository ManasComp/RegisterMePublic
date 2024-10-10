namespace RegisterMe.Application.Cages.Dtos.RentedCage;

public class AbstractRentedCageDtoDto
{
    public required List<int> ExhibitionDaysId { get; init; }
    public required int Length { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
}
