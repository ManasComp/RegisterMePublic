#region

using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;

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
                .ForMember(x => x.Price,
                    opt => opt.MapFrom(x =>
                        new MultiCurrencyPrice(x.Amounts.Where(y => y.Currency == Currency.Czk).Single().Amount,
                            x.Amounts.Where(y => y.Currency == Currency.Eur).Single().Amount)));
        }
    }
}
