#region

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

#endregion

namespace WebGui.Areas.Visitor.Models.PriceModels;

public class PriceModelStep3
{
    [ValidateNever] public List<ExhibitionDaysWithPrice> PriceDays { get; init; } = [];

    public required int ExhibitionId { get; init; }
}
