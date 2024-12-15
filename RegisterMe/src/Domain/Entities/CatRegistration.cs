#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class CatRegistration : BaseAuditableEntity
{
    public required string? Note { get; set; }
    public virtual ExhibitedCat? ExhibitedCat { get; set; }
    public virtual Litter? Litter { get; set; }
    public required int RegistrationToExhibitionId { get; init; }

    [ForeignKey(nameof(RegistrationToExhibitionId))]
    public virtual RegistrationToExhibition RegistrationToExhibition { get; init; } = null!;

    public virtual ICollection<CatDay> CatDays { get; set; } = new List<CatDay>();
}
