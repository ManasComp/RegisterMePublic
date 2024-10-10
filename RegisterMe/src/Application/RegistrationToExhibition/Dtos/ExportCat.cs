// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace RegisterMe.Application.RegistrationToExhibition.Dtos;

public class ExportCat
{
    public required string Name { get; init; }
    public required string Sex { get; init; }
    public required string? PedigreeNumber { get; init; }
    public required string? Breeder { get; init; }
    public required string Breed { get; init; }
    public required string Variant { get; init; }
    public required string? Group { get; init; }
    public required string Class { get; init; }
    public required string DateOfBirth { get; init; }
    public required ExportParent? Mother { get; init; }
    public required ExportParent? Father { get; init; }
}
