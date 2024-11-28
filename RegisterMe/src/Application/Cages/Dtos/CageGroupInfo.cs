#region

using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Cages.Dtos;

public record CageGroupInfo
{
    public required decimal TotalNumberOfRentedCages { get; init; }
    public required CagesRec NumberOfUsedCages { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required int Length { get; init; }
    public required Dictionary<RentedType, HashSet<int>> AvailableTypes { get; init; } = null!;
}
