#region

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class StringCheckBox
{
    [ValidateNever] public required string Id { get; init; } = null!;
    [ValidateNever] public required string LabelName { get; init; } = null!;
    [ValidateNever] public required bool IsChecked { get; set; }
}
