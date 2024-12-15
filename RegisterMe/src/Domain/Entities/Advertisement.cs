#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class Advertisement : BaseEntity
{
    public required string Description { get; init; } = null!;
    public virtual List<Amounts> Amounts { get; init; } = null!;
    public required bool IsDefault { get; init; }
    public required int ExhibitionId { get; set; }
    [ForeignKey(nameof(ExhibitionId))] public virtual Exhibition Exhibition { get; init; } = null!;

    public virtual ICollection<RegistrationToExhibition> PersonRegistrations { get; } =
        new List<RegistrationToExhibition>();
}
