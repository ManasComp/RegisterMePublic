#region

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RulesEngine.Models;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class UpdateDiscountMethodsModel
{
    public int ExhibitionId { get; init; }
    public int? DiscountId { get; init; }

    [ValidateNever] public Workflow DiscountWorkflow { get; init; } = null!;

    public string DiscountWorkflowString { get; init; } = null!;
}
