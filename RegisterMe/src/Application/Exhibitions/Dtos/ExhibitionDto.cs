#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record ExhibitionDto
{
    public required DateOnly ExhibitionStart { get; init; }
    public required DateOnly ExhibitionEnd { get; init; }
    public required string Location { get; init; } = null!;
    public required string Name { get; init; } = null!;
    public required int Id { get; init; }
    public required bool IsCancelled { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Exhibition, ExhibitionDto>()
                .ForMember(x => x.ExhibitionStart,
                    opt => opt.MapFrom(x => x.Days.Select(exhibitionDay => exhibitionDay.Date).Min()))
                .ForMember(x => x.ExhibitionEnd,
                    opt => opt.MapFrom(x => x.Days.Select(exhibitionDay => exhibitionDay.Date).Max()))
                .ForMember(x => x.Location, opt => opt.MapFrom(x => x.Address.StreetAddress));
        }
    }
}
