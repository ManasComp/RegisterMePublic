namespace RegisterMe.Application.Organizations.Dtos;

public record UpdateOrganizationDto
{
    public required string Name { get; init; } = null!;
    public required string Email { get; init; } = null!;
    public required string TelNumber { get; init; } = null!;
    public required string Website { get; init; } = null!;
    public required string Address { get; init; } = null!;
    public required int Id { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateOrganizationDto, UpdateOrganizationDto>()
                .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address))
                .ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}
