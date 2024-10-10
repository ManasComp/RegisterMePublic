namespace RegisterMe.Application.Services.Groups;

public class GroupService
{
    private readonly List<GroupDto> _groups;

    public GroupService(GroupInitializer groupInitializer)
    {
        _groups = groupInitializer.GetGroups().Select(x => new GroupDto
        {
            Filter = x.Filter, Name = x.Name, GroupId = x.GroupId
        }).ToList();
    }

    public GroupDto GetGroupById(string groupId)
    {
        return _groups.First(x => x.GroupId == groupId);
    }
}
