namespace RegisterMe.Application.Cages.Dtos.RentedCage;

public class RentedRentedCageDto : AbstractRentedCageDtoDto
{
    public required int Id { get; init; }
    public required List<RentedTypeDto> RentedCageTypes { get; init; } = null!;

    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<Domain.Entities.RentedCage, RentedRentedCageDto>()
                .ForMember(x => x.RentedCageTypes,
                    opt => opt.MapFrom(x =>
                        x.RentedTypes.Select(y => new RentedTypeDto { Id = y.Id, RentedType = y.RentedType })))
                .ForMember(x => x.ExhibitionDaysId, opt => opt.MapFrom(x => x.ExhibitionDays.Select(y => y.Id)));
        }
    }
}
