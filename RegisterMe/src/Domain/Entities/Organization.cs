namespace RegisterMe.Domain.Entities;

public class Organization : BaseAuditableEntity
{
    public required string Name { get; set; } = null!;
    public required string Email { get; set; } = null!;
    public required string Ico { get; init; } = null!;
    public required string TelNumber { get; set; } = null!;
    public required string Website { get; set; } = null!;
    public virtual List<ApplicationUser> Administrator { get; set; } = null!;
    public required string Address { get; set; } = null!;
    public virtual IEnumerable<Exhibition> Exhibitions { get; init; } = new List<Exhibition>();
    public required bool IsConfirmed { get; set; }
}
