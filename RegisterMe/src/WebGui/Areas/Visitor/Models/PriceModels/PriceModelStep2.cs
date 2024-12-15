#region

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

#endregion

namespace WebGui.Areas.Visitor.Models.PriceModels;

public class PriceModelStep2
{
    [ValidateNever] public List<IntCheckBox> Days { get; init; } = [];

    public required int ExhibitionId { get; init; }

    public required string? PricesIds { get; init; }
}
