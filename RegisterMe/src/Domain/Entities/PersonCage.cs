#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class PersonCage : BaseEntity
{
    public required int Length { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public virtual List<CatDay> CatDays { get; init; } = null!;
    public required int RegistrationToExhibitionId { get; init; }

    [ForeignKey(nameof(RegistrationToExhibitionId))]
    public virtual RegistrationToExhibition RegistrationToExhibition { get; init; } = null!;
}
