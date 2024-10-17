#region

using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Cages.Dtos.RentedCage;

public class CreateRentedRentedCageDto : AbstractRentedCageDtoDto
{
    public required List<RentedType> RentedCageTypes { get; init; } = null!;
    public required int Count { get; init; }

    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<CreateRentedRentedCageDto, Domain.Entities.RentedCage>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.RentedTypes,
                    src => src.MapFrom(x =>
                        x.RentedCageTypes.Select(rentedType => new RentedTypeEntity { RentedType = rentedType })
                            .ToList()))
                .ForMember(x => x.ExhibitionDays, opt => opt.Ignore());
        }
    }
}
