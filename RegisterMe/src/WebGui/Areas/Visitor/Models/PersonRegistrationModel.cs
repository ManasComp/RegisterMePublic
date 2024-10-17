#region

using Microsoft.AspNetCore.Mvc.Rendering;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Commands.CreateRegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Domain.Entities;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class PersonRegistrationModel
{
    public required int? RegistrationToExhibitionId { get; set; }
    public required int ExhibitionId { get; init; }
    public required int ExhibitorId { get; init; }
    public required string City { get; init; } = null!;
    public required string Street { get; init; } = null!;
    public required string HouseNumber { get; init; } = null!;
    public required string ZipCode { get; init; } = null!;
    public required string Organization { get; init; } = null!;
    public required string MemberNumber { get; init; } = null!;
    public required string FirstName { get; init; } = null!;
    public required string LastName { get; init; } = null!;
    public required string Country { get; init; } = null!;
    public required DateOnly DateOfBirth { get; init; }
    public required string Email { get; init; } = null!;
    public required string PhoneNumber { get; init; } = null!;
    public string Address => Street + " " + HouseNumber + ", " + City + ", " + ZipCode + ", " + Country;
    public required int SelectedAdvertisementId { get; set; }
    public required IEnumerable<SelectListItem> Advertisements { get; init; } = new List<SelectListItem>();
    public required bool Disabled { get; set; } = true;
    public required string EmailToOrganization { get; set; } = null!;
    public required bool IsPartOfCsch { get; init; }
    public required bool IsPartOfFife { get; init; }


    public static PersonRegistrationModel Initialize(ApplicationUser user, ExhibitorAndUserDto exhibitorAndUser,
        List<AdvertisementDto> advertisements, int exhibitionId, bool disabled)
    {
        PersonRegistrationModel personRegistrationModel = new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber!, // asp net user has this optional, but we require it, even in db
            Email = user.Email!, // asp net user has this optional, but we require it, even in db
            DateOfBirth = user.DateOfBirth,
            Organization = exhibitorAndUser.Organization,
            MemberNumber = exhibitorAndUser.MemberNumber,
            Street = exhibitorAndUser.Street,
            HouseNumber = exhibitorAndUser.HouseNumber,
            City = exhibitorAndUser.City,
            Country = exhibitorAndUser.Country,
            Advertisements = ConvertToVm(advertisements),
            SelectedAdvertisementId = advertisements.First(x => x.IsDefault).Id,
            ExhibitorId = exhibitorAndUser.Id,
            ExhibitionId = exhibitionId,
            ZipCode = exhibitorAndUser.ZipCode,
            Disabled = disabled,
            RegistrationToExhibitionId = null,
            IsPartOfCsch = exhibitorAndUser.IsPartOfCsch,
            EmailToOrganization = exhibitorAndUser.EmailToOrganization,
            IsPartOfFife = exhibitorAndUser.IsPartOfFife
        };

        return personRegistrationModel;
    }

    public static PersonRegistrationModel Initialize(RegistrationToExhibitionDto registrationToExhibitionDto,
        IEnumerable<AdvertisementDto> advertisements, int exhibitionId, int exhibitorId, bool disabled)
    {
        PersonRegistrationModel personRegistrationModel = new()
        {
            Advertisements = ConvertToVm(advertisements),
            SelectedAdvertisementId = registrationToExhibitionDto.AdvertisementId,
            ExhibitorId = exhibitorId,
            ExhibitionId = exhibitionId,
            Disabled = disabled,
            RegistrationToExhibitionId = registrationToExhibitionDto.Id,
            City = registrationToExhibitionDto.PersonRegistration.City,
            Street = registrationToExhibitionDto.PersonRegistration.Street,
            HouseNumber = registrationToExhibitionDto.PersonRegistration.HouseNumber,
            ZipCode = registrationToExhibitionDto.PersonRegistration.ZipCode,
            Organization = registrationToExhibitionDto.PersonRegistration.Organization,
            MemberNumber = registrationToExhibitionDto.PersonRegistration.MemberNumber,
            FirstName = registrationToExhibitionDto.PersonRegistration.FirstName,
            LastName = registrationToExhibitionDto.PersonRegistration.LastName,
            Country = registrationToExhibitionDto.PersonRegistration.Country,
            DateOfBirth = registrationToExhibitionDto.PersonRegistration.DateOfBirth,
            Email = registrationToExhibitionDto.PersonRegistration.Email,
            PhoneNumber = registrationToExhibitionDto.PersonRegistration.PhoneNumber,
            IsPartOfCsch = registrationToExhibitionDto.PersonRegistration.IsPartOfCsch,
            EmailToOrganization = registrationToExhibitionDto.PersonRegistration.EmailToOrganization,
            IsPartOfFife = registrationToExhibitionDto.PersonRegistration.IsPartOfFife
        };

        return personRegistrationModel;
    }

    private static IEnumerable<SelectListItem> ConvertToVm(IEnumerable<AdvertisementDto> advertisements)
    {
        return advertisements.Select(y => new SelectListItem { Text = y.Description, Value = y.Id.ToString() });
    }

    public CreateRegistrationToExhibitionCommand ConvertToCreateRegistrationToExhibitionCommand(int? advertisementId)
    {
        return new CreateRegistrationToExhibitionCommand
        {
            RegistrationToExhibition = new CreateRegistrationToExhibitionDto
            {
                ExhibitionId = ExhibitionId,
                ExhibitorId = ExhibitorId,
                AdvertisementId = SelectedAdvertisementId
            }
        };
    }
}
