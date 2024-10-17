#region

using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record AdvertisementDto : UpsertAdvertisementDto
{
    public required int Id { get; init; }
    public int ExhibitionId { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Advertisement, AdvertisementDto>()
                .ForMember(x => x.Price, opt => opt.MapFrom(x => new MultiCurrencyPrice(x.PriceCzk, x.PriceEur)));
        }
    }
}
