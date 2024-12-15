#region

using System.ComponentModel.DataAnnotations;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Organizations.Dtos;

public record CreateOrganizationDto
{
    public required string Name { get; init; } = null!;
    [EmailAddress] public required string Email { get; init; } = null!;
    public required string Ico { get; init; } = null!;
    public required string TelNumber { get; init; } = null!;
    public required string Website { get; init; } = null!;
    public required string Address { get; init; } = null!;
    public required string AdminId { get; init; } = null!;

    public static CreateOrganizationDto CreateBlankOrganizationDto(string userId)
    {
        return new CreateOrganizationDto
        {
            Email = "",
            Ico = "",
            Name = "",
            TelNumber = "",
            Website = "",
            Address = "",
            AdminId = userId
        };
    }

    // ReSharper disable once UnusedType.Local
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateOrganizationDto, Organization>()
                .ForMember(x => x.IsConfirmed, opt => opt.MapFrom(x => false))
                .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address))
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Exhibitions, opt => opt.Ignore())
                .ForMember(x => x.Administrator, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore());
        }
    }
}
