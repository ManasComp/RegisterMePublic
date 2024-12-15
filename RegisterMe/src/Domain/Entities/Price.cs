namespace RegisterMe.Domain.Entities;

public class Price : BaseEntity
{
    public virtual List<Amounts> Amounts { get; init; } = null!;
    public virtual List<ExhibitionDay> ExhibitionDays { get; init; } = [];
    public virtual List<Group> Groups { get; init; } = [];
}
