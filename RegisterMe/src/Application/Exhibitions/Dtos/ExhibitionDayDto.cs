#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record ExhibitionDayDto
{
    public required int Id { get; init; }
    public required DateOnly Date { get; init; }
    public required List<int> CagesForRentIds { get; init; } = [];

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ExhibitionDay, ExhibitionDayDto>()
                .ForMember(x => x.CagesForRentIds, opt => opt.MapFrom(src => src.CagesForRent.Select(x => x.Id)));
        }
    }
}
