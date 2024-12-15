#region

using RegisterMe.Application.CatRegistrations.Dtos;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class PrefillModel
{
    public required List<CatModelP> Cats { get; init; } = [];
    public required int RegistrationToExhibitionId { get; init; }
}
