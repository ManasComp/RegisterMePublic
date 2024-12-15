#region

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class EmsModel : StepModel
{
    [Required] public string Ems { get; init; } = null!;

    [Required] public string Breed { get; init; } = null!;

    [Remote(areaName: "Visitor", controller: "Registration", action: "VerifyColour",
        AdditionalFields = nameof(Ems))]
    public string? Colour { get; init; }
}
