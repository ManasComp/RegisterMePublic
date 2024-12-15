namespace RegisterMe.Application.Services.Groups;

public record GroupDto : DatabaseGroupDto
{
    public Func<GroupInitializer.FilterParameter, bool> Filter { get; init; } = null!;
}
