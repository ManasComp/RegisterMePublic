#region

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

#endregion

namespace WebGui.Areas.Visitor.Models.PriceModels;

public class PriceModelStep1
{
    [ValidateNever] public List<StringCheckBox> AvailableGroups { get; init; } = [];

    public required int ExhibitionId { get; init; }
}
