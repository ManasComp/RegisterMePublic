#region

using RegisterMe.Application.CatRegistrations.Enums;

#endregion

namespace WebGui.Areas.Visitor.Models;

public record SummaryHeaderPartialHeader
{
    public required RegistrationType RegistrationType { get; init; }
    public required int RegistrationToExhibitionId { get; init; }
    public required bool Disabled { get; init; }
    public required int? CatRegistrationId { get; init; }
    public required bool IsLitter { get; init; }
}
