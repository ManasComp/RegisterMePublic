#region

using RegisterMe.Application.Cages;

#endregion

namespace RegisterMe.Application.CatRegistrations.Dtos;

public record CatDayDto : CreateCatDayDto
{
    public required int Id { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<MiddleCatDayDto, CatDayDto>()
                .ForMember(x => x.RentedCageTypeId, opt => opt.Ignore())
                .AfterMap<GetCageHashIsAreRegisteredTo>();
        }
    }
}

public class GetCageHashIsAreRegisteredTo(ICagesService cagesService) : IMappingAction<MiddleCatDayDto, CatDayDto>
{
    public void Process(MiddleCatDayDto source, CatDayDto destination, ResolutionContext context)
    {
        destination.RentedCageTypeId =
            cagesService.GetCageHashIsAreRegisteredTo(source.Id, source.ExhibitionDayId, source.RentedCageTypeId)
                .Result;
    }
}
