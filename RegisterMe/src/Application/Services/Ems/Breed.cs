namespace RegisterMe.Application.Services.Ems;

public record Breed : EmsCodePartDto
{
    public required bool RequiresGroup { get; init; }
}
