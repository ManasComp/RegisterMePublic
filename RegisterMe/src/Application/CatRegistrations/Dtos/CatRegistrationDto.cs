namespace RegisterMe.Application.CatRegistrations.Dtos;

public record CatRegistrationDto : AbstractCatRegistrationDto
{
    public required int Id { get; init; }
    public required int RegistrationToExhibitionId { get; init; }
    public required List<CatDayDto> CatDays { get; init; } = [];

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<MiddleCatRegistrationDto, CatRegistrationDto>()
                .ForMember(x => x.CatDays, opt => opt.MapFrom(src => src.CatDays))
                .ForMember(x => x.Litter, opt => opt.MapFrom(src => src.Litter))
                .ForMember(x => x.ExhibitedCat, opt => opt.MapFrom(src => src.ExhibitedCat));
        }
    }
}
