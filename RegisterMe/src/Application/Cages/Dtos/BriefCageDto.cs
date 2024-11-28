#region

using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Cages.Dtos;

public record BriefCageDto
{
    public required string Ids { get; init; } = null!;
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required int Length { get; init; }
    public required List<RentedType> RentedTypes { get; init; } = [];
    public required List<SmallExhibitionDayDto> ExhibitionDays { get; init; } = [];
    public required int Count { get; init; }
}
