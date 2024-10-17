#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class ExhibitionDay : BaseEntity
{
    public required DateOnly Date { get; init; }
    public virtual List<RentedCage> CagesForRent { get; init; } = [];
    public required int ExhibitionId { get; init; }
    public virtual List<Price> Prices { get; init; } = [];
    [ForeignKey(nameof(ExhibitionId))] public virtual Exhibition Exhibition { get; init; } = null!;
    public virtual List<CatDay> CatDays { get; init; } = [];
}
