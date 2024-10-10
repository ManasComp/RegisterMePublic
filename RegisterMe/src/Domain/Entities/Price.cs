namespace RegisterMe.Domain.Entities;

public class Price : BaseEntity
{
    public required decimal PriceKc { get; init; }
    public required decimal PriceEur { get; init; }
    public virtual List<ExhibitionDay> ExhibitionDays { get; init; } = [];
    public virtual List<Group> Groups { get; init; } = [];
}
