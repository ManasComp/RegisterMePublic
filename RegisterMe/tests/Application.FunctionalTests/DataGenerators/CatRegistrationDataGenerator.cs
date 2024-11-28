#region

using RegisterMe.Application.Cages;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.DataGenerators;

public class CatRegistrationDataGenerator
{
    public static CreateLitterDto CreateLitter = new()
    {
        PassOfOrigin = "zazadano",
        BirthDate = DateOnly.FromDateTime(DateTime.Now).AddMonths(-9),
        NameOfBreedingStation = "BreedingStation",
        Breed = "dfdf",
        Mother =
            new MotherDto
            {
                TitleAfterName = "fdf",
                TitleBeforeName = "fdf",
                Name = "Father",
                Ems = "PER",
                PedigreeNumber = "Fajkjkther",
                Colour = "Father",
                Breed = "Father"
            },
        Father = new FatherDto
        {
            TitleAfterName = "fdf",
            TitleBeforeName = "fdf",
            Name = "Father",
            Ems = "PER",
            PedigreeNumber = "Fathjkjker",
            Colour = "Father",
            Breed = "Father"
        },
        Breeder = new BreederDto
        {
            FirstName = "Father", LastName = "Father", Country = "CZ", BreederIsSameAsExhibitor = false
        }
    };

    public static CreateExhibitedCatDto CreateExhibitedCat = new()
    {
        Neutered = false,
        TitleAfterName = "",
        TitleBeforeName = "",
        Name = "ExhibitedCat",
        Ems = "PER",
        PedigreeNumber = "ExhibitedCat",
        BirthDate = new DateOnly(2021, 1, 1),
        Colour = "ExhibitedCat",
        Breed = "ExhibitedCat",
        Group = null,
        Sex = Gender.Female,
        Breeder =
            new BreederDto
            {
                FirstName = "ExhibitedCat",
                LastName = "ExhibitedCat",
                Country = "SK",
                BreederIsSameAsExhibitor = false
            },
        Father = new FatherDto
        {
            TitleAfterName = "ExhibitedCat",
            TitleBeforeName = "ExhibitedCat",
            Name = "ExhibitedCat",
            Ems = "PER",
            PedigreeNumber = "ExhibhhhitedCat",
            Colour = "ExhibitedCat",
            Breed = "ExhibitedCat"
        },
        Mother = new MotherDto
        {
            TitleAfterName = "ExhibitedCat",
            TitleBeforeName = "ExhibitedCat",
            Name = "ExhibitedCat",
            Ems = "PER",
            PedigreeNumber = "ExhibighhtedCat",
            Colour = "ExhibitedCat",
            Breed = "ExhibitedCat"
        }
    };

    public static CreateCatRegistrationDto WithoutDays(int registrationToExhibitionId)
    {
        CreateCatRegistrationDto catRegistration = new()
        {
            Litter = null,
            CatDays = [],
            ExhibitedCat = new CreateExhibitedCatDto
            {
                Group = null,
                Neutered = true,
                TitleAfterName = "Test Title",
                TitleBeforeName = "Test Title",
                Name = "Test Name",
                Ems = "Test Ems",
                PedigreeNumber = "Test PedigreeNumber",
                BirthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-1)),
                Colour = "Test Colour",
                Breed = "Test Breed",
                Sex = Gender.Female,
                Breeder =
                    new BreederDto
                    {
                        FirstName = "Test FirstName",
                        LastName = "Test LastName",
                        Country = "Test Country",
                        BreederIsSameAsExhibitor = true
                    },
                Father = new FatherDto
                {
                    TitleAfterName = "Tfest Title",
                    TitleBeforeName = "Tesft Title",
                    Name = "Tefst Name",
                    Ems = "Tesft Ems",
                    PedigreeNumber = "Tefst PedigreeNumber",
                    Colour = "Tefst Colour",
                    Breed = "Tfest Breed"
                },
                Mother = new MotherDto
                {
                    TitleAfterName = "Tesmt Title",
                    TitleBeforeName = "Temst Title",
                    Name = "Tmest Name",
                    Ems = "Temst Ems",
                    PedigreeNumber = "Temst PedigreeNumber",
                    Colour = "Tmest Colour",
                    Breed = "Temst Breed"
                }
            },
            Note = "Test Note",
            RegistrationToExhibitionId = registrationToExhibitionId
        };
        return catRegistration;
    }

    public static CreateCatRegistrationDto WithDays(int registrationToExhibitionId,
        List<ExhibitionDayDto> exhibitionDays,
        RentedCageGroup? cageHash, CreateCageDto? createCageDto)
    {
        CreateCatRegistrationDto catRegistration = new()
        {
            Litter = null,
            CatDays =
            [
                new CreateCatDayDto
                {
                    RentedCageTypeId = cageHash,
                    Cage = createCageDto,
                    ExhibitorsCage = null,
                    ExhibitionDayId = exhibitionDays.First().Id,
                    GroupsIds = ["15"]
                },
                new CreateCatDayDto
                {
                    RentedCageTypeId = cageHash,
                    Cage = createCageDto,
                    ExhibitorsCage = null,
                    ExhibitionDayId = exhibitionDays[1].Id,
                    GroupsIds = ["15"]
                }
            ],
            ExhibitedCat = new CreateExhibitedCatDto
            {
                Group = null,
                Neutered = true,
                TitleAfterName = "Test Title",
                TitleBeforeName = "Test Title",
                Name = "Test Name",
                Ems = "PER",
                PedigreeNumber = "Test PedigreeNumber",
                BirthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-1)),
                Colour = "Test Colour",
                Breed = "PER",
                Sex = Gender.Female,
                Breeder =
                    new BreederDto
                    {
                        FirstName = "mTest FirstName",
                        LastName = "Test LastName",
                        Country = "CZ",
                        BreederIsSameAsExhibitor = true
                    },
                Father = new FatherDto
                {
                    TitleAfterName = "fTest Title",
                    TitleBeforeName = "Tfest Title",
                    Name = "fTest Name",
                    Ems = "PER",
                    PedigreeNumber = "fTest PedigreeNumber",
                    Colour = "fTest Colour",
                    Breed = "PER"
                },
                Mother = new MotherDto
                {
                    TitleAfterName = "mTest Title",
                    TitleBeforeName = "mTest Title",
                    Name = "mTest Name",
                    Ems = "PER",
                    PedigreeNumber = "mTest PedigreeNumber",
                    Colour = "mTest Colour",
                    Breed = "PER"
                }
            },
            Note = "Test Note",
            RegistrationToExhibitionId = registrationToExhibitionId
        };
        return catRegistration;
    }

    public static CreateCatRegistrationDto WithDays1(int registrationToExhibitionId,
        List<ExhibitionDayDto> exhibitionDays,
        RentedCageGroup? cageHash, CreateCageDto? createCageDto)
    {
        CreateCatRegistrationDto catRegistration = new()
        {
            Litter = null,
            CatDays =
            [
                new CreateCatDayDto
                {
                    RentedCageTypeId = cageHash,
                    Cage = createCageDto,
                    ExhibitorsCage = null,
                    ExhibitionDayId = exhibitionDays.First().Id,
                    GroupsIds = ["15"]
                }
            ],
            ExhibitedCat = new CreateExhibitedCatDto
            {
                Group = null,
                Neutered = true,
                TitleAfterName = "1Test Title",
                TitleBeforeName = "1Test Title",
                Name = "1Test Name",
                Ems = "1Test Ems",
                PedigreeNumber = "1Test PedigreeNumber",
                BirthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-2)),
                Colour = "1Test Colour",
                Breed = "PER",
                Sex = Gender.Male,
                Breeder =
                    new BreederDto
                    {
                        FirstName = "1Test FirstName",
                        LastName = "1Test LastName",
                        Country = "SK",
                        BreederIsSameAsExhibitor = false
                    },
                Father = new FatherDto
                {
                    TitleAfterName = "father Title",
                    TitleBeforeName = "father1Test Title",
                    Name = "father1Test Name",
                    Ems = "father1Test Ems",
                    PedigreeNumber = "1fatherTest PedigreeNumber",
                    Colour = "father1Test Colour",
                    Breed = "PER"
                },
                Mother = new MotherDto
                {
                    TitleAfterName = "mother 1Test Title",
                    TitleBeforeName = "mother 1Test Title",
                    Name = "mother 1Test Name",
                    Ems = "mother 1Test Ems",
                    PedigreeNumber = "Tmother 1est PedigreeNumber",
                    Colour = "mother 1Test Colour",
                    Breed = "PER"
                }
            },
            Note = "1Test Note",
            RegistrationToExhibitionId = registrationToExhibitionId
        };
        return catRegistration;
    }
}
