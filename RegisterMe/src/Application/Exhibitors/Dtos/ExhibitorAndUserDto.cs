#region

using Microsoft.AspNetCore.Identity;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Exhibitors.Dtos;

public class ExhibitorAndUserDto : UpsertExhibitorDto
{
    [ProtectedPersonalData] public required string PhoneNumber { get; init; } = null!;
    [ProtectedPersonalData] public required DateOnly DateOfBirth { get; init; }
    [ProtectedPersonalData] public required string FirstName { get; init; } = null!;
    [ProtectedPersonalData] public required string LastName { get; init; } = null!;
    [ProtectedPersonalData] public required string Email { get; init; } = null!;
    [PersonalData] public required int Id { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Exhibitor, ExhibitorAndUserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AspNetUser.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AspNetUser.PhoneNumber))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
                .ForMember(dest => dest.HouseNumber, opt => opt.MapFrom(src => src.HouseNumber))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ZipCode))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.AspNetUser.DateOfBirth))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AspNetUser.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AspNetUser.LastName));
        }
    }
}
