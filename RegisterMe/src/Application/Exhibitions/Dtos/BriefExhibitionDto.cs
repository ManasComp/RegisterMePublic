#region

using RegisterMe.Application.Exhibitions.Enums;
using RegisterMe.Application.Services.Workflows;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record BriefExhibitionDto : AbstractExhibitionDto
{
    public required int Id { get; init; }
    public required AddressDto Address { get; init; }
    public required ExhibitionStatus Status { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Exhibition, BriefExhibitionDto>()
                .ForMember(x => x.RegistrationEnd, opt => opt.MapFrom(x => x.RegistrationEnd))
                .ForMember(x => x.RegistrationStart, opt => opt.MapFrom(x => x.RegistrationStart))
                .ForMember(x => x.ExhibitionStart,
                    opt => opt.MapFrom(x => x.Days.Select(exhibitionDay => exhibitionDay.Date).Min()))
                .ForMember(x => x.ExhibitionEnd,
                    opt => opt.MapFrom(x => x.Days.Select(exhibitionDay => exhibitionDay.Date).Max()))
                .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address))
                .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address))
                .ForMember(x => x.ExhibitionStart,
                    opt => opt.MapFrom(x => x.Days.Select(exhibitionDay => exhibitionDay.Date).Min()))
                .ForMember(x => x.ExhibitionEnd,
                    opt => opt.MapFrom(x => x.Days.Select(exhibitionDay => exhibitionDay.Date).Max()))
                .ForMember(x => x.BankAccount, opt => opt.MapFrom(x => x.BankAccount))
                .ForMember(x => x.Status, opt => opt.Ignore());
        }
    }
}
