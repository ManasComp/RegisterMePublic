#region

using RegisterMe.Application.Cages.Dtos;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class AdminIndex
{
    public required int ExhibitionId { get; init; }
    public required List<ExhibitionCagesInfo> ExhibitionCagesInfos { get; init; } = null!;
    public required List<RegistrationToExhibitionAdminIndexModel> Registrations { get; init; } = null!;

    public required bool HasDrafts { get; init; }
    public required int DeleteAfterHours { get; init; }
}
