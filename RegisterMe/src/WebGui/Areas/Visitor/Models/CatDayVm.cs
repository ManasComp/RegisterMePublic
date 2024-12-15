#region

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class CatDayVm : IValidatableObject
{
    [ValidateNever] public required List<SelectListItem> GroupsYouCanRegisterTo { get; set; } = null!;
    [ValidateNever] public required List<SelectListItem> ExistingCages { get; set; }
    public required int ExhibitionDayId { get; init; }
    public bool Attendance { get; init; }
    public required string? SelectedCage { get; init; }
    public required int Width { get; init; }
    public required int Length { get; init; }
    public required List<string>? SelectedGroupsPerDay { get; init; }
    public required bool SelectDefaultCage { get; init; }
    [ValidateNever] public required string Date { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Attendance && (SelectedGroupsPerDay == null || SelectedGroupsPerDay.Count == 0))
        {
            yield return new ValidationResult("Musíte vybrat alespoň jednu skupinu",
                [nameof(SelectedGroupsPerDay)]);
        }
    }
}
