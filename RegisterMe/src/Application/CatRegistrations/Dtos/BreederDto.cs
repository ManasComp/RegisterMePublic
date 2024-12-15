#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.CatRegistrations.Dtos;

public record BreederDto
{
    public required string FirstName { get; set; } = null!;
    public required string LastName { get; set; } = null!;
    public required string Country { get; set; } = null!;
    public required bool BreederIsSameAsExhibitor { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Breeder, BreederDto>();
            CreateMap<BreederDto, Breeder>()
                .ForMember(x => x.ExhibitedCatId, opt => opt.Ignore())
                .ForMember(x => x.LitterId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(x => x.ExhibitedCat, opt => opt.Ignore())
                .ForMember(x => x.Litter, opt => opt.Ignore());
        }
    }
}
