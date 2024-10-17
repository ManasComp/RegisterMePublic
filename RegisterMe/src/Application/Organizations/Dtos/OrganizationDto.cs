#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Organizations.Dtos;

public record OrganizationDto
{
    public required string Name { get; init; } = null!;
    public required string Email { get; init; } = null!;
    public required string Ico { get; init; } = null!;
    public required string TelNumber { get; init; } = null!;
    public required string Website { get; init; } = null!;
    public required string Address { get; init; } = null!;
    public required int Id { get; init; }
    public required List<int> ExhibitionIds { get; set; }
    public required bool IsConfirmed { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Organization, OrganizationDto>()
                .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address))
                .ForMember(x => x.ExhibitionIds, opt => opt.MapFrom(x => x.Exhibitions.Select(e => e.Id).ToList()));
            CreateMap<CreateOrganizationDto, OrganizationDto>()
                .ForMember(x => x.IsConfirmed, opt => opt.MapFrom(x => false))
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.ExhibitionIds, opt => opt.Ignore());
        }
    }
}
