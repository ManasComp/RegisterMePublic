// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace WebGui.Areas.Visitor.Models;

public class DeleteCagesCommand
{
    public string CagesId { get; init; } = null!;
    public int ExhibitionId { get; init; }
}
