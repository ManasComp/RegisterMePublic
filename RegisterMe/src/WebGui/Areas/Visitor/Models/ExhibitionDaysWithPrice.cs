#region

using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class ExhibitionDaysWithPrice
{
    public required List<SmallExhibitionDayDto> ExhibitionDays { get; init; } = [];
    public required decimal PriceCzk { get; init; }
    public required decimal PriceEur { get; init; }
}
