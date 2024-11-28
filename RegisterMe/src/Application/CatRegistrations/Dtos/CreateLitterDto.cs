#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.CatRegistrations.Dtos;

public record CreateLitterDto
{
    public required string? PassOfOrigin { get; set; }
    public required DateOnly BirthDate { get; set; }
    public required string NameOfBreedingStation { get; set; } = null!;
    public required string Breed { get; set; } = null!;
    public required BreederDto Breeder { get; init; } = null!;
    public required FatherDto Father { get; set; }
    public required MotherDto Mother { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateLitterDto, Litter>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CatRegistration, opt => opt.Ignore())
                .ForMember(x => x.CatRegistrationId, opt => opt.Ignore());

            CreateMap<Litter, CreateLitterDto>()
                .ForMember(x => x.Breeder, opt => opt.MapFrom(src => src.Breeder))
                .ForMember(x => x.Father, opt => opt.MapFrom(src => src.Father))
                .ForMember(x => x.Mother, opt => opt.MapFrom(src => src.Mother));
        }
    }
}
