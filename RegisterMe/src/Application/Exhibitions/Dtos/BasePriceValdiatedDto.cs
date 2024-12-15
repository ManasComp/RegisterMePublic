namespace RegisterMe.Application.Exhibitions.Dtos;

public record BasePriceValidatedDto
{
    public required List<string> GroupsIds { get; init; }
    public required List<PriceDays> PriceDays { get; init; }
}
