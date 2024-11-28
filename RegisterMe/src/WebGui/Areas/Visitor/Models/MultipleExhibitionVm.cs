namespace WebGui.Areas.Visitor.Models;

public class MultipleExhibitionVm : StepModel
{
    public required List<CatDayVm> DayDetails { get; init; } = null!;
    public required string? Note { get; init; }
    public required bool IsConfirmation { get; init; }
}
