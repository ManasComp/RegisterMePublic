namespace RegisterMe.Application.Exhibitions.Dtos;

public class BigPriceDto
{
    public required string PriceIds { get; init; }
    public required string Groups { get; init; } = null!;
    public required List<SmallPriceDto> Prices { get; init; } = [];
    public required int ExhibitionId { get; init; }
}
