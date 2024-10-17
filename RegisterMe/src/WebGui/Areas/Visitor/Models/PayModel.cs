#region

using RegisterMe.Domain.Enums;

#endregion

namespace WebGui.Areas.Visitor.Models;

public record PayModel
{
    public int RegistrationToExhibitionId { get; init; }
    public Currency Currency { get; init; }
}
