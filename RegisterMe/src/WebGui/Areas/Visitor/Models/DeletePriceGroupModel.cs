// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace WebGui.Areas.Visitor.Models;

public class DeletePriceGroupModel
{
    public string PriceIds { get; init; } = null!;
    public int ExhibitionId { get; init; }
}
