#region

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class IntCheckBox
{
    [ValidateNever] public required int Id { get; init; }
    [ValidateNever] public required string LabelName { get; init; } = null!;
    [ValidateNever] public required bool IsChecked { get; set; }
}
