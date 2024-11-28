// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace RegisterMe.Application.RegistrationToExhibition.Dtos;

public class ExportPerson
{
    public required string Name { get; init; }
    public required string Country { get; init; }
    public required string MemberOf { get; init; }
    public required List<ExportCat> Cats { get; init; }
}
