namespace RegisterMe.Application.Services.Groups;

public record DatabaseGroupDto
{
    public required string Name { get; set; } = null!;
    public required string GroupId { get; init; } = null!;
}
