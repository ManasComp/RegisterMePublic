namespace RegisterMe.Domain.Entities;

public class Parent : BaseEntity
{
    public required string? TitleBeforeName { get; init; }
    public required string? TitleAfterName { get; init; }
    public required string? Name { get; init; }
    public required string? Ems { get; init; }
    public required string? PedigreeNumber { get; init; }
    public required string? Colour { get; init; }
    public required string? Breed { get; init; }

    public required int? ExhibitedCatIsMotherOfId { get; init; }
    public required int? ExhibitedCatIsFatherOfId { get; init; }
    public required int? LitterIsMotherOfId { get; init; }
    public required int? LitterIsFatherOfId { get; init; }
}
