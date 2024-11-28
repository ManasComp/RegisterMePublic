namespace WebGui.Areas.Visitor.Models;

public class RegistrationToExhibitionAdminIndexModel
{
    public required string Status { get; init; } = null!;
    public required string Name { get; init; } = null!;
    public required string Surname { get; init; } = null!;
    public required int NumberOfCats { get; init; }
    public required string MemberNumber { get; init; } = null!;
    public required string Organization { get; init; } = null!;
    public required bool IsPaid { get; init; }
    public required int RegistrationToExhibitionId { get; init; }
    public required bool IsSend { get; init; }
    public required string ExhibitionName { get; init; } = null!;
}
