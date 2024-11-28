#region

using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record SmallPriceDto
{
    public required MultiCurrencyPrice Price { get; init; }
    public required List<SmallExhibitionDayDto> ExhibitionDays { get; init; } = [];
    public required List<string> Groups { get; init; } = [];
    public required int Id { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Price, SmallPriceDto>()
                .ForMember(x => x.Groups, opt => opt.MapFrom(opt1 => opt1.Groups.Select(x => x.GroupId)))
                .ForMember(x => x.ExhibitionDays, opt => opt.MapFrom(opt2 => opt2.ExhibitionDays)
                )
                .ForMember(x => x.Price, opt => opt.MapFrom(opt3 => new MultiCurrencyPrice
                (
                    opt3.Amounts.Where(y => y.Currency == Currency.Czk).Single().Amount,
                    opt3.Amounts.Where(y => y.Currency == Currency.Eur).Single().Amount)));
        }
    }
}
