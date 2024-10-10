#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class RentedTypeEntity : BaseEntity
{
    public int CageId { get; init; }
    [ForeignKey(nameof(CageId))] public virtual RentedCage RentedCage { get; init; } = null!;
    public required RentedType RentedType { get; init; }
    public virtual List<CatDay> CatDays { get; init; } = null!;
}
