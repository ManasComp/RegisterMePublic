#region

using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Cages.Dtos.Cage;

public record CageGroupDescriptorAttributes
{
    public required int Height { get; init; }
    public required int Length { get; init; }
    public required int Width { get; init; }
    public required RentedType RentedType { get; init; }
    public required RentingType CageType { get; init; }
}
