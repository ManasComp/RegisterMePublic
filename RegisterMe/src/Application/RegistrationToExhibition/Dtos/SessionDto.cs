// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace RegisterMe.Application.RegistrationToExhibition.Dtos;

public record SessionDto
{
    public required string Url { get; init; }
    public required string Id { get; set; }
}
