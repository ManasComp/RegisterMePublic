#region

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class SummaryVm : StepModel
{
    [ValidateNever] public required TemporaryCatRegistrationDto CatRegistrationDto { get; init; }

    [ValidateNever] public required PersonRegistrationDto PersonRegistrationDto { get; init; }
}
