#region

using RegisterMe.Application.CatRegistrations.Enums;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class StepModel
{
    public required int Step { get; init; }

    /// <summary>
    ///     Indication if cat is home cat
    /// </summary>
    public required RegistrationType RegistrationType { get; init; }

    public required int RegistrationToExhibitionId { get; init; }

    /// <summary>
    ///     Indication if form is editable
    /// </summary>
    public required bool Disabled { get; init; }
}
