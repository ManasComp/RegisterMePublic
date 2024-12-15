#region

using RegisterMe.Application.Cages;
using RegisterMe.Application.Cages.Dtos.Cage;

#endregion

namespace RegisterMe.Application.CatRegistrations.Dtos;

public record CreateCatDayDto
{
    public required RentedCageGroup? RentedCageTypeId { get; set; }
    public required int? ExhibitorsCage { get; init; }
    public required int ExhibitionDayId { get; init; }
    public required CreateCageDto? Cage { get; init; }
    public required List<string> GroupsIds { get; init; } = [];

    public virtual bool Equals(CreateCatDayDto? other)
    {
        if (other is null)
        {
            return false;
        }

        return EqualityComparer<RentedCageGroup?>.Default.Equals(RentedCageTypeId, other.RentedCageTypeId) &&
               ExhibitorsCage == other.ExhibitorsCage &&
               ExhibitionDayId == other.ExhibitionDayId &&
               EqualityComparer<CreateCageDto?>.Default.Equals(Cage, other.Cage) &&
               GroupsIds.SequenceEqual(other.GroupsIds);
    }

    public override int GetHashCode()
    {
        // this is ok because changing is done only in mapping (where object is constructed)
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        int hash = HashCode.Combine(RentedCageTypeId, ExhibitorsCage, ExhibitionDayId, Cage);
        return GroupsIds.Aggregate(hash, HashCode.Combine);
    }
}
