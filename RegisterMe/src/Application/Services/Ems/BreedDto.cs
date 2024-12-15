namespace RegisterMe.Application.Services.Ems;

public record BreedDto : EmsCodePartPerCatTypeDto
{
    public BreedDto(Breed attribute, Status status) : base(attribute, status)
    {
        RequiresGroup = attribute.RequiresGroup;
    }

    public bool RequiresGroup { get; init; }
}
