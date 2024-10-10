#region

using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.CatRegistrations.Dtos;

public record FatherDto : ParentDto
{
    public override Gender Sex => Gender.Male;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Parent, FatherDto>();
            CreateMap<FatherDto, Parent>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.ExhibitedCatIsFatherOfId, opt => opt.Ignore())
                .ForMember(x => x.ExhibitedCatIsMotherOfId, opt => opt.Ignore())
                .ForMember(x => x.LitterIsFatherOfId, opt => opt.Ignore())
                .ForMember(x => x.LitterIsMotherOfId, opt => opt.Ignore());
        }
    }
}
