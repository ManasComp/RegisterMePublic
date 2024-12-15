namespace RegisterMe.Application.Cages.Dtos;

public record ExhibitionCagesInfo
{
    public required List<CageGroupInfo> CageGroupInfos { get; init; } = [];
    public required int NumberOfPersonCages { get; init; }
    public required string Date { get; init; } = null!;
    public required int ExhibitionDayId { get; init; }
}
