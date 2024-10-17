#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class CatDay : BaseEntity
{
    public required int? RentedCageTypeId { get; init; }
    [ForeignKey(nameof(RentedCageTypeId))] public virtual RentedTypeEntity? RentedTypeEntity { get; init; }
    public required int? ExhibitorsCage { get; init; }
    [ForeignKey(nameof(ExhibitorsCage))] public virtual PersonCage? Cage { get; init; }
    public required int ExhibitionDayId { get; init; }
    [ForeignKey(nameof(ExhibitionDayId))] public virtual ExhibitionDay ExhibitionDay { get; init; } = null!;
    public required int CatRegistrationId { get; init; }

    [ForeignKey(nameof(CatRegistrationId))]
    public virtual CatRegistration CatRegistration { get; init; } = null!;

    public virtual List<Group> Groups { get; init; } = [];
}
