namespace RegisterMe.Application.CatRegistrations.Dtos;

public record UpdateCatRegistrationDto : AbstractCatRegistrationDto
{
    public required int Id { get; init; }
    public required List<CreateCatDayDto> CatDays { get; init; } = [];

    // ReSharper disable once UnusedType.Local
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdateCatRegistrationDto, CreateCatRegistrationDto>()
                .ForMember(dest => dest.RegistrationToExhibitionId, opt => opt.Ignore());
        }
    }
}
