#region

using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record UpsertAdvertisementDto
{
    public required string Description { get; init; } = null!;
    public required MultiCurrencyPrice Price { get; init; }
    public required bool IsDefault { get; set; }


    // ReSharper disable once UnusedType.Local
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpsertAdvertisementDto, Advertisement>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Exhibition, opt => opt.Ignore())
                .ForMember(x => x.ExhibitionId, opt => opt.Ignore())
                .ForMember(x => x.PersonRegistrations, opt => opt.Ignore())
                .ForMember(x => x.Amounts,
                    opt => opt.MapFrom(x => new List<Amounts>
                    {
                        new() { Currency = Currency.Czk, Amount = x.Price.PriceCzk },
                        new() { Currency = Currency.Eur, Amount = x.Price.PriceEur }
                    }));
        }
    }
}
