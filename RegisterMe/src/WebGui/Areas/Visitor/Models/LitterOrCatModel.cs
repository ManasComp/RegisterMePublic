namespace WebGui.Areas.Visitor.Models;

public class LitterOrCatModel
{
    public required int RegistrationToExhibitionId { get; init; }
    public required bool IsTemporary { get; init; }
}
