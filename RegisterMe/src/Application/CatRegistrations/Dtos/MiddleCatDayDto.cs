#region

using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.CatRegistrations.Dtos;

public class MiddleCatDayDto
{
    public required int Id { get; init; }
    public required int? RentedCageTypeId { get; init; }
    public required int? ExhibitorsCage { get; init; }
    public required CreateCageDto? Cage { get; init; }
    public int ExhibitionDayId { get; init; }
    public IReadOnlyCollection<string> GroupsIds { get; init; } = new List<string>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CatDay, MiddleCatDayDto>()
                .ForMember(x => x.GroupsIds, opt => opt.MapFrom(src => src.Groups.Select(x => x.GroupId)))
                .ForMember(x => x.Cage, opt => opt.MapFrom(src => src.Cage));
        }
    }
}
