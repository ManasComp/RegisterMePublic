#region

using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.CatRegistrations.Dtos;

public record CreateExhibitedCatDto
{
    public required bool Neutered { get; set; }
    public required string? TitleBeforeName { get; set; }
    public required string? TitleAfterName { get; set; }
    public required string Name { get; set; } = null!;
    public required string Ems { get; set; } = null!;
    public required string? PedigreeNumber { get; set; }
    public required DateOnly BirthDate { get; set; }
    public required string? Colour { get; set; }
    public required string Breed { get; set; } = null!;
    public required int? Group { get; set; }
    public required Gender Sex { get; set; }
    public required BreederDto? Breeder { get; set; }
    public required FatherDto? Father { get; set; }
    public required MotherDto? Mother { get; set; }

    public string FullName => $"{TitleBeforeName} {Name} {TitleAfterName}".Trim();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateExhibitedCatDto, ExhibitedCat>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CatRegistration, opt => opt.Ignore())
                .ForMember(x => x.CatRegistrationId, opt => opt.Ignore())
                .ForMember(x => x.Group, opt => opt.MapFrom(src => src.Group));
            CreateMap<ExhibitedCat, CreateExhibitedCatDto>()
                .ForMember(x => x.Breeder, opt => opt.MapFrom(src => src.Breeder))
                .ForMember(x => x.Father, opt => opt.MapFrom(src => src.Father))
                .ForMember(x => x.Mother, opt => opt.MapFrom(src => src.Mother))
                .ForMember(x => x.Group, opt => opt.MapFrom(src => src.Group));
        }
    }
}
