#region

using RegisterMe.Application.Common.Utils;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Cages.Dtos;

public class ItemDto : IEquatable<ItemDto>
{
    private readonly ListComparer<int> _intComparer = new();
    private readonly ListComparer<RentedType> _rentedTypeComparer = new();
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required int Length { get; init; }
    public required List<RentedType> RentedTypes { get; init; } = null!;
    public List<SmallExhibitionDayDto> ExhibitionDays { get; init; } = null!;

    public bool Equals(ItemDto? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Width == other.Width &&
               Height == other.Height &&
               Length == other.Length &&
               _rentedTypeComparer.Equals(RentedTypes, other.RentedTypes) &&
               _intComparer.Equals(ExhibitionDays.Select(x => x.Id).ToList(),
                   other.ExhibitionDays.Select(x => x.Id).ToList());
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == GetType() && Equals((ItemDto)obj);
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Width);
        hash.Add(Height);
        hash.Add(Length);
        foreach (RentedType rentedType in RentedTypes)
        {
            hash.Add(rentedType);
        }

        foreach (SmallExhibitionDayDto? day in ExhibitionDays)
        {
            hash.Add(day.Id);
        }

        return hash.ToHashCode();
    }
}
