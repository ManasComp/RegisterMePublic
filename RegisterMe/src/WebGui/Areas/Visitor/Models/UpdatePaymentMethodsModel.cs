#region

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RulesEngine.Models;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class UpdatePaymentMethodsModel
{
    public required int ExhibitionId { get; init; }
    [ValidateNever] public required Workflow PaymentTypes { get; init; } = null!;
    public string PaymentTypesString { get; init; } = null!;
    public required bool CanPayByCard { get; init; }
}
