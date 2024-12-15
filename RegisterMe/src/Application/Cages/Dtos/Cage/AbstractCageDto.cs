#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Cages.Dtos.Cage;

public record CreateCageDto
{
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required int Length { get; init; }

    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<PersonCage, CreateCageDto>();
        }
    }
}
