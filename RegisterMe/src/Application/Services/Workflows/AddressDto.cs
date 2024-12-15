#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Services.Workflows;

public record AddressDto
{
    public required string StreetAddress { get; init; } = null!;
    public required string Latitude { get; init; } = null!;
    public required string Longitude { get; init; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Address, AddressDto>();
            CreateMap<AddressDto, Address>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Exhibition, opt => opt.Ignore())
                .ForMember(x => x.ExhibitionId, opt => opt.Ignore());
        }
    }
}
