#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record SmallExhibitionDayDto
{
    public required int Id { get; init; }
    public required DateOnly Date { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ExhibitionDay, SmallExhibitionDayDto>();
        }
    }
}
