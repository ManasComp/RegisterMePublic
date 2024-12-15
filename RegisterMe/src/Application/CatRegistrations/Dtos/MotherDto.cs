#region

using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.CatRegistrations.Dtos;

public record MotherDto : ParentDto
{
    public override Gender Sex => Gender.Female;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Parent, MotherDto>();
            CreateMap<MotherDto, Parent>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.ExhibitedCatIsFatherOfId, opt => opt.Ignore())
                .ForMember(x => x.ExhibitedCatIsMotherOfId, opt => opt.Ignore())
                .ForMember(x => x.LitterIsFatherOfId, opt => opt.Ignore())
                .ForMember(x => x.LitterIsMotherOfId, opt => opt.Ignore());
        }
    }
}
