#region

using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests;

public static class CompareUtils
{
    public static bool Equals(CreateOrganizationDto createOrganizationDto, UpdateOrganizationDto updateOrganizationDto)
    {
        return createOrganizationDto.Name == updateOrganizationDto.Name &&
               createOrganizationDto.Email == updateOrganizationDto.Email &&
               createOrganizationDto.TelNumber == updateOrganizationDto.TelNumber &&
               createOrganizationDto.Website == updateOrganizationDto.Website &&
               createOrganizationDto.Address == updateOrganizationDto.Address;
    }

    public static bool Equals(CreateCatRegistrationDto createCatRegistrationDto, CatRegistrationDto catRegistration)
    {
        return createCatRegistrationDto.RegistrationToExhibitionId == catRegistration.RegistrationToExhibitionId &&
               createCatRegistrationDto.Note == catRegistration.Note &&
               ((createCatRegistrationDto.ExhibitedCat == null && catRegistration.ExhibitedCat == null) ||
                (createCatRegistrationDto.ExhibitedCat != null &&
                 createCatRegistrationDto.ExhibitedCat.Equals(catRegistration.ExhibitedCat))) &&
               ((createCatRegistrationDto.Litter == null && catRegistration.Litter == null) ||
                (createCatRegistrationDto.Litter != null &&
                 createCatRegistrationDto.Litter.Equals(catRegistration.Litter)));
    }

    public static bool Equals(UpdateCatRegistrationDto createCatRegistrationDto, CatRegistrationDto catRegistration)
    {
        return createCatRegistrationDto.Note == catRegistration.Note &&
               ((createCatRegistrationDto.ExhibitedCat == null && catRegistration.ExhibitedCat == null) ||
                (createCatRegistrationDto.ExhibitedCat != null &&
                 createCatRegistrationDto.ExhibitedCat.Equals(catRegistration.ExhibitedCat))) &&
               ((createCatRegistrationDto.Litter == null && catRegistration.Litter == null) ||
                (createCatRegistrationDto.Litter != null &&
                 createCatRegistrationDto.Litter.Equals(catRegistration.Litter)));
    }

    public static bool Equals(CatRegistrationDto catRegistration, UpdateCatRegistrationDto createCatRegistrationDto)
    {
        return Equals(createCatRegistrationDto, catRegistration);
    }


    public static bool Equals(CatRegistrationDto createOrganizationDto, CreateCatRegistrationDto updateOrganizationDto)
    {
        return Equals(updateOrganizationDto, createOrganizationDto);
    }


    public static bool Equals(UpdateOrganizationDto updateOrganizationDto, CreateOrganizationDto createOrganizationDto)
    {
        return Equals(createOrganizationDto, updateOrganizationDto);
    }

    public static bool Equals(UpdateOrganizationDto updateOrganizationDto, OrganizationDto createOrganizationDto)
    {
        return createOrganizationDto.Name == updateOrganizationDto.Name &&
               createOrganizationDto.Email == updateOrganizationDto.Email &&
               createOrganizationDto.TelNumber == updateOrganizationDto.TelNumber &&
               createOrganizationDto.Website == updateOrganizationDto.Website &&
               createOrganizationDto.Address == updateOrganizationDto.Address;
    }

    public static bool Equals(OrganizationDto updateOrganizationDto, UpdateOrganizationDto createOrganizationDto)
    {
        return Equals(createOrganizationDto, updateOrganizationDto);
    }

    public static bool Equals(CreateOrganizationDto updateOrganizationDto, OrganizationDto createOrganizationDto)
    {
        return createOrganizationDto.Name == updateOrganizationDto.Name &&
               createOrganizationDto.Email == updateOrganizationDto.Email &&
               createOrganizationDto.TelNumber == updateOrganizationDto.TelNumber &&
               createOrganizationDto.Website == updateOrganizationDto.Website &&
               createOrganizationDto.Address == updateOrganizationDto.Address;
    }

    public static bool Equals(UpdateExhibitionDto updateOrganizationDto, BriefExhibitionDto createOrganizationDto)
    {
        return createOrganizationDto.Name == updateOrganizationDto.Name &&
               createOrganizationDto.Email == updateOrganizationDto.Email &&
               createOrganizationDto.Phone == updateOrganizationDto.Phone &&
               createOrganizationDto.Url == updateOrganizationDto.Url &&
               createOrganizationDto.Description == updateOrganizationDto.Description &&
               createOrganizationDto.RegistrationStart == updateOrganizationDto.RegistrationStart &&
               createOrganizationDto.RegistrationEnd == updateOrganizationDto.RegistrationEnd &&
               createOrganizationDto.ExhibitionStart == updateOrganizationDto.ExhibitionStart &&
               createOrganizationDto.ExhibitionEnd == updateOrganizationDto.ExhibitionEnd &&
               createOrganizationDto.Address.Equals(updateOrganizationDto.Address);
    }

    public static bool Equals(BriefExhibitionDto updateOrganizationDto, UpdateExhibitionDto createOrganizationDto)
    {
        return Equals(createOrganizationDto, updateOrganizationDto);
    }

    public static bool Equals(CreateExhibitionDto updateOrganizationDto, BriefExhibitionDto createOrganizationDto)
    {
        return createOrganizationDto.Name == updateOrganizationDto.Name &&
               createOrganizationDto.Email == updateOrganizationDto.Email &&
               createOrganizationDto.Phone == updateOrganizationDto.Phone &&
               createOrganizationDto.Url == updateOrganizationDto.Url &&
               createOrganizationDto.Description == updateOrganizationDto.Description &&
               createOrganizationDto.RegistrationStart == updateOrganizationDto.RegistrationStart &&
               createOrganizationDto.RegistrationEnd == updateOrganizationDto.RegistrationEnd &&
               createOrganizationDto.ExhibitionStart == updateOrganizationDto.ExhibitionStart &&
               createOrganizationDto.ExhibitionEnd == updateOrganizationDto.ExhibitionEnd &&
               createOrganizationDto.Address.Equals(updateOrganizationDto.Address);
    }

    public static bool Equals(BriefExhibitionDto updateOrganizationDto, CreateExhibitionDto createOrganizationDto)
    {
        return Equals(createOrganizationDto, updateOrganizationDto);
    }

    public static bool Equals(UpsertAdvertisementDto upsertAdvertisementDto, AdvertisementDto briefAdvertisementDto)
    {
        return upsertAdvertisementDto.Description == briefAdvertisementDto.Description &&
               upsertAdvertisementDto.Price.Equals(briefAdvertisementDto.Price);
    }

    public static bool Equals(CreateRegistrationToExhibitionDto createRegistrationToExhibitionDto,
        RegistrationToExhibitionDto registrationToExhibition)
    {
        return createRegistrationToExhibitionDto.ExhibitionId == registrationToExhibition.ExhibitionId &&
               createRegistrationToExhibitionDto.ExhibitorId == registrationToExhibition.ExhibitorId &&
               createRegistrationToExhibitionDto.AdvertisementId == registrationToExhibition.AdvertisementId;
    }

    public static bool Equals(RegistrationToExhibitionDto createRegistrationToExhibitionDto,
        CreateRegistrationToExhibitionDto registrationToExhibition)
    {
        return Equals(registrationToExhibition, createRegistrationToExhibitionDto);
    }

    public static bool Equals(AdvertisementDto briefAdvertisementDto, UpsertAdvertisementDto upsertAdvertisementDto)
    {
        return Equals(upsertAdvertisementDto, briefAdvertisementDto);
    }
}
