#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class Breeder : BaseEntity
{
    public required bool BreederIsSameAsExhibitor { get; init; }
    public required string FirstName { get; init; } = null!;
    public required string LastName { get; init; } = null!;
    public required string Country { get; init; } = null!;

    public required int? ExhibitedCatId { get; init; }
    [ForeignKey(nameof(ExhibitedCatId))] public virtual ExhibitedCat? ExhibitedCat { get; init; }

    public required int? LitterId { get; init; }
    [ForeignKey(nameof(LitterId))] public virtual Litter? Litter { get; init; }
}
