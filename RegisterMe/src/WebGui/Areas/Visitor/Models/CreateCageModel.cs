namespace WebGui.Areas.Visitor.Models;

public class CreateCageModel
{
    public required List<IntCheckBox> ExhibitionDaysId { get; set; }
    public required List<IntCheckBox> RentedCageTypes { get; set; } = null!;
    public required int Length { get; set; }
    public required int Width { get; set; }
    public required int Height { get; set; }
    public required int Count { get; set; }
    public required int ExhibitionId { get; init; }
    public string? CagesIds { get; set; }
}
