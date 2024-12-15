#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class Address : BaseEntity
{
    public required string StreetAddress { get; init; } = null!;
    public required string Latitude { get; init; } = null!;
    public required string Longitude { get; init; } = null!;
    public int ExhibitionId { get; init; }
    [ForeignKey(nameof(ExhibitionId))] public virtual Exhibition Exhibition { get; init; } = null!;
}
