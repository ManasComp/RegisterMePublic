#region

using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record PriceDto
{
    public required int Id { get; init; }
    public required MultiCurrencyPrice Price { get; init; }
    public required List<SmallExhibitionDayDto> ExhibitionDays { get; init; } = [];
    public required List<string> Groups { get; init; } = [];


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Price, PriceDto>()
                .ForMember(x => x.Groups, opt => opt.MapFrom(price => price.Groups.Select(x => x.GroupId)))
                .ForMember(x => x.ExhibitionDays, opt => opt.MapFrom(price => price.ExhibitionDays))
                .ForMember(x => x.Price, opt => opt.MapFrom(price => new MultiCurrencyPrice
                (
                    price.Amounts.Where(y => y.Currency == Currency.Czk).Single().Amount,
                    price.Amounts.Where(y => y.Currency == Currency.Eur).Single().Amount)));
        }
    }
}
