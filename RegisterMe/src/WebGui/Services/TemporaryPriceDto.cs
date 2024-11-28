#region

using WebGui.Areas.Visitor.Models;

#endregion

namespace WebGui.Services;

public class TemporaryPriceDto
{
    public required List<StringCheckBox> Groups { get; init; } = [];
    public List<IntCheckBox> Days { get; init; } = [];
    public required string? OriginalPricesIds { get; init; }
}
