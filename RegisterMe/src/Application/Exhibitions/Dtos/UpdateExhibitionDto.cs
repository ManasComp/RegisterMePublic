#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record UpdateExhibitionDto : BaseExhibitionValidatedDto
{
    public required int Id { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BriefExhibitionDto, UpdateExhibitionDto>();
            CreateMap<Exhibition, UpdateExhibitionDto>()
                .ForMember(x => x.ExhibitionStart,
                    opt => opt.MapFrom(x => x.Days.Select(exhibitionDay => exhibitionDay.Date).Min()))
                .ForMember(x => x.ExhibitionEnd,
                    opt => opt.MapFrom(x => x.Days.Select(exhibitionDay => exhibitionDay.Date).Max()))
                .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address));
            CreateMap<UpdateExhibitionDto, Exhibition>()
                .ForMember(x => x.IsPublished, opt => opt.Ignore())
                .ForMember(x => x.OrganizationId, opt => opt.Ignore())
                .ForMember(x => x.IsCancelled, opt => opt.Ignore())
                .ForMember(x => x.Address, opt => opt.Ignore())
                .ForMember(x => x.Days, opt => opt.Ignore())
                .ForMember(x => x.Advertisements, opt => opt.Ignore())
                .ForMember(x => x.Workflows, opt => opt.Ignore())
                .ForMember(x => x.PaymentTypesWorkflow, opt => opt.Ignore())
                .ForMember(x => x.RegistrationsToExhibitions, opt => opt.Ignore())
                .ForMember(x => x.Organization, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore());
        }
    }
}
