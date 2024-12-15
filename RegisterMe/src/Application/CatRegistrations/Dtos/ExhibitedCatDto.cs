#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.CatRegistrations.Dtos;

public record ExhibitedCatDto : CreateExhibitedCatDto
{
    public required int Id { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ExhibitedCat, ExhibitedCatDto>()
                .ForMember(x => x.Breeder, opt => opt.MapFrom(src => src.Breeder))
                .ForMember(x => x.Father, opt => opt.MapFrom(src => src.Father))
                .ForMember(x => x.Mother, opt => opt.MapFrom(src => src.Mother));

            CreateMap<ExhibitedCatDto, ExhibitedCat>()
                .ForMember(x => x.Breeder, opt => opt.MapFrom(src => src.Breeder))
                .ForMember(x => x.Father, opt => opt.MapFrom(src => src.Father))
                .ForMember(x => x.Mother, opt => opt.MapFrom(src => src.Mother))
                .ForMember(x => x.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(x => x.CatRegistration, opt => opt.Ignore())
                .ForMember(x => x.CatRegistrationId, opt => opt.Ignore());
        }
    }
}
