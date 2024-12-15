namespace RegisterMe.Domain.Entities;

public class RentedCage : BaseEntity
{
    public required int Length { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public virtual List<RentedTypeEntity> RentedTypes { get; set; } = null!;
    public virtual List<ExhibitionDay> ExhibitionDays { get; set; } = null!;
}
