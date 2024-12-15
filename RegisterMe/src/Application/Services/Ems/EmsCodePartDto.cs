namespace RegisterMe.Application.Services.Ems;

public record EmsCodePartDto
{
    public required string Code { get; init; } = null!;
    public required List<string> PotentialCodePartInCzech { get; init; } = null!;
}
