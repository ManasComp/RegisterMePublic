#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Exhibitions.Dtos;

public record CreateExhibitionDto : BaseExhibitionValidatedDto
{
    public required int OrganizationId { get; init; }

    // ReSharper disable once UnusedType.Local
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateExhibitionDto, Exhibition>()
                .ForMember(x => x.IsPublished, opt => opt.MapFrom(x => false))
                .ForMember(x => x.Advertisements, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address))
                .ForMember(x => x.Organization, opt => opt.Ignore())
                .ForMember(x => x.Workflows, opt => opt.Ignore())
                .ForMember(x => x.PaymentTypesWorkflow, opt => opt.Ignore())
                .ForMember(x => x.RegistrationsToExhibitions, opt => opt.Ignore())
                .ForMember(x => x.Days, opt => opt.Ignore())
                .ForMember(x => x.IsCancelled, opt => opt.MapFrom(x => false));
        }
    }
}
