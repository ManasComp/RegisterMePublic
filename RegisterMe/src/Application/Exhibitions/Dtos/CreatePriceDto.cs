#region

using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record CreatePriceDto
{
    public required MultiCurrencyPrice Price { get; init; }
    public required List<int> ExhibitionDayIds { get; init; } = [];
    public required List<string> Groups { get; init; } = [];

    // ReSharper disable once UnusedType.Local
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreatePriceDto, Price>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Groups, opt => opt.Ignore())
                .ForMember(x => x.ExhibitionDays, opt => opt.Ignore())
                .ForMember(x => x.Amounts,
                    opt => opt.MapFrom(x => new List<Amounts>
                    {
                        new() { Currency = Currency.Czk, Amount = x.Price.PriceCzk },
                        new() { Currency = Currency.Eur, Amount = x.Price.PriceEur }
                    }));
        }
    }
}
