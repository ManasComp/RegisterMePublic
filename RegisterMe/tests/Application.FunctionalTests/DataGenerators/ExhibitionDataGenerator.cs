#region

using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Services.Workflows;

#endregion

namespace RegisterMe.Application.FunctionalTests.DataGenerators;

public static class ExhibitionDataGenerator
{
    public static CreateExhibitionDto Exhibition1(int organizationId)
    {
        CreateExhibitionDto exhibition = new()
        {
            BankAccount = "123456789",
            Iban = "123456789012345678912341",
            Phone = "123456789",
            Email = "test@example.com",
            Name = "Annual Art Expo",
            Url = "https://www.artexpo.com",
            Description = "A showcase of contemporary art and sculpture",
            RegistrationStart = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
            RegistrationEnd = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ExhibitionStart = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            ExhibitionEnd = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
            OrganizationId = organizationId,
            Address = new AddressDto
            {
                Latitude = "Test North", StreetAddress = "Test Street", Longitude = "Test South"
            },
            DeleteNotFinishedRegistrationsAfterHours = 12
        };

        return exhibition;
    }

    public static CreateExhibitionDto OutdatedRegistration(int organizationId)
    {
        CreateExhibitionDto exhibition = new()
        {
            BankAccount = "123456789",
            Iban = "123412345678901234567891",
            Phone = "123456789",
            Email = "test@example.com",
            Name = "Annual Art Expo",
            Url = "https://www.artexpo.com",
            Description = "A showcase of contemporary art and sculpture",
            RegistrationStart = DateOnly.FromDateTime(DateTime.Now.AddDays(-7)),
            RegistrationEnd = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)),
            ExhibitionStart = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
            ExhibitionEnd = DateOnly.FromDateTime(DateTime.Now.AddDays(0)),
            OrganizationId = organizationId,
            Address = new AddressDto
            {
                Latitude = "Test North", StreetAddress = "Test Street", Longitude = "Test South"
            },
            DeleteNotFinishedRegistrationsAfterHours = 15
        };

        return exhibition;
    }

    public static UpdateExhibitionDto UpdatedExhibition(CreateExhibitionDto exhibition, int id)
    {
        return new UpdateExhibitionDto
        {
            BankAccount = exhibition.BankAccount,
            Iban = exhibition.Iban,
            Phone = exhibition.Phone,
            Email = exhibition.Email,
            Name = exhibition.Name,
            Url = exhibition.Url,
            Description = exhibition.Description,
            RegistrationStart = exhibition.RegistrationStart,
            RegistrationEnd = exhibition.RegistrationEnd,
            ExhibitionStart = exhibition.ExhibitionStart,
            ExhibitionEnd = exhibition.ExhibitionEnd,
            Address = exhibition.Address,
            Id = id,
            DeleteNotFinishedRegistrationsAfterHours = 24
        };
    }

    public static CreateExhibitionDto Exhibition2(int organizationId)
    {
        CreateExhibitionDto exhibition = new()
        {
            BankAccount = "987654321",
            Iban = "987654321987654321123411",
            Phone = "987654321",
            Email = "test1@example.com",
            Name = "Annual1 Art Expo",
            Url = "https://www.artexpo1.com",
            Description = "A showcase1 of contemporary art and sculpture",
            RegistrationStart = new DateOnly(2025, 2, 2),
            RegistrationEnd = new DateOnly(2025, 2, 11),
            ExhibitionStart = new DateOnly(2025, 2, 16),
            ExhibitionEnd = new DateOnly(2025, 2, 18),
            OrganizationId = organizationId,
            Address = new AddressDto
            {
                Latitude = "Test1 North", StreetAddress = "Test1 Street", Longitude = "Test1 South"
            },
            DeleteNotFinishedRegistrationsAfterHours = 1
        };

        return exhibition;
    }
}
