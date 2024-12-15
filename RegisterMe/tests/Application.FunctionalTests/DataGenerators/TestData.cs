#region

using RegisterMe.Application.Cages;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.DataGenerators;

public class TestData
{
    public enum Breeders
    {
        Breeder1,
        Breeder2
    }

    public enum CatDays
    {
        NineDoubleCage,
        NineDoubleCageShared1,
        NineDoubleCageShared2,
        CatDays2,
        BothDays,
        Single,
        Personal,
        SharedPersonal
    }

    public enum ExhibitedCats
    {
        ExhibitedCat1,
        ExhibitedCat2
    }

    public enum Fathers
    {
        Father1,
        Father2
    }

    public enum Litters
    {
        Litter1,
        Litter2
    }

    public enum Mothers
    {
        Mother1,
        Mother2
    }

    public BreederDto GetBreederDto(Breeders breeder)
    {
        return breeder switch
        {
            Breeders.Breeder1 => new BreederDto
            {
                FirstName = "breeder1", LastName = "breeder1", Country = "CZ", BreederIsSameAsExhibitor = false
            },
            Breeders.Breeder2 => new BreederDto
            {
                FirstName = "breeder2", LastName = "breeder2", Country = "SK", BreederIsSameAsExhibitor = false
            },
            _ => throw new ArgumentOutOfRangeException(nameof(breeder), breeder, null)
        };
    }

    public FatherDto GetFatherDto(Fathers father)
    {
        return father switch
        {
            Fathers.Father1 => new FatherDto
            {
                TitleAfterName = "father1",
                TitleBeforeName = "father1",
                Name = "father1",
                Ems = "PER",
                PedigreeNumber = "father1",
                Colour = "father1",
                Breed = "father1"
            },
            Fathers.Father2 => new FatherDto
            {
                TitleAfterName = "father2",
                TitleBeforeName = "father2",
                Name = "father2",
                Ems = "PER",
                PedigreeNumber = "father2",
                Colour = "father2",
                Breed = "father2"
            },
            _ => throw new ArgumentOutOfRangeException(nameof(father), father, null)
        };
    }

    public MotherDto GetMotherDto(Mothers mother)
    {
        return mother switch
        {
            Mothers.Mother1 => new MotherDto
            {
                TitleAfterName = "mother1",
                TitleBeforeName = "mother1",
                Name = "mother1",
                Ems = "PER",
                PedigreeNumber = "mother1",
                Colour = "mother1",
                Breed = "mother1"
            },
            Mothers.Mother2 => new MotherDto
            {
                TitleAfterName = "mother2",
                TitleBeforeName = "mother2",
                Name = "mother2",
                Ems = "PER",
                PedigreeNumber = "mother2",
                Colour = "mother2",
                Breed = "mother2"
            },
            _ => throw new ArgumentOutOfRangeException(nameof(mother), mother, null)
        };
    }

    public CreateExhibitedCatDto GetExhibitedCatDto(ExhibitedCats exhibitedCat, Breeders breeders, Fathers? fathers,
        Mothers? mothers)
    {
        BreederDto breeder = GetBreederDto(breeders);
        FatherDto? father = fathers.HasValue ? GetFatherDto(fathers.Value) : null;
        MotherDto? mother = mothers.HasValue ? GetMotherDto(mothers.Value) : null;

        return exhibitedCat switch
        {
            ExhibitedCats.ExhibitedCat1 => new CreateExhibitedCatDto
            {
                Neutered = false,
                TitleAfterName = "",
                TitleBeforeName = "",
                Name = "exhibitedCat1",
                Ems = "PER",
                PedigreeNumber = "exhibitedCat1",
                BirthDate = new DateOnly(2021, 1, 1),
                Colour = "exhibitedCat1",
                Breed = "exhibitedCat1",
                Group = null,
                Sex = Gender.Female,
                Breeder = breeder,
                Father = father,
                Mother = mother
            },
            ExhibitedCats.ExhibitedCat2 => new CreateExhibitedCatDto
            {
                Neutered = false,
                TitleAfterName = "",
                TitleBeforeName = "",
                Name = "exhibitedCat2",
                Ems = "PER",
                PedigreeNumber = "exhibitedCat2",
                BirthDate = new DateOnly(2021, 1, 1),
                Colour = "exhibitedCat2",
                Breed = "exhibitedCat2",
                Group = null,
                Sex = Gender.Female,
                Breeder = breeder,
                Father = father,
                Mother = mother
            },
            _ => throw new ArgumentOutOfRangeException(nameof(exhibitedCat), exhibitedCat, null)
        };
    }

    public CreateLitterDto GetLitterDto(Litters litter, Breeders breeders, Fathers fathers, Mothers mothers)
    {
        BreederDto breeder = GetBreederDto(breeders);
        FatherDto father = GetFatherDto(fathers);
        MotherDto mother = GetMotherDto(mothers);

        return litter switch
        {
            Litters.Litter1 => new CreateLitterDto
            {
                BirthDate = DateOnly.FromDateTime(DateTime.Now).AddMonths(-8),
                Breed = "exhibitedCat1",
                Breeder = breeder,
                Father = father,
                Mother = mother,
                PassOfOrigin = null,
                NameOfBreedingStation = "BreedingStation"
            },
            Litters.Litter2 => new CreateLitterDto
            {
                BirthDate = DateOnly.FromDateTime(DateTime.Now).AddMonths(-6),
                Breed = "exhibitedCat2",
                Breeder = breeder,
                Father = father,
                Mother = mother,
                PassOfOrigin = "zazadano",
                NameOfBreedingStation = "BreedingStation"
            },
            _ => throw new ArgumentOutOfRangeException(nameof(litter), litter, null)
        };
    }

    public List<CreateCatDayDto> GetCatDayDto(CatDays catDay, List<int> exhibitionDaysId, int? personCage = null)
    {
        return catDay switch
        {
            CatDays.SharedPersonal =>
            [
                new CreateCatDayDto
                {
                    RentedCageTypeId = null,
                    ExhibitorsCage = personCage,
                    ExhibitionDayId = exhibitionDaysId.First(),
                    GroupsIds = ["9"],
                    Cage = null
                }
            ],

            CatDays.Personal =>
            [
                new CreateCatDayDto
                {
                    RentedCageTypeId = null,
                    ExhibitorsCage = null,
                    ExhibitionDayId = exhibitionDaysId.First(),
                    GroupsIds = ["9"],
                    Cage = new CreateCageDto { Width = 120, Height = 120, Length = 120 }
                }
            ],

            CatDays.NineDoubleCage =>
            [
                new CreateCatDayDto
                {
                    RentedCageTypeId = new RentedCageGroup("120;120;120;Double;RentedWithZeroOtherCats"),
                    ExhibitorsCage = null,
                    ExhibitionDayId = exhibitionDaysId.First(),
                    GroupsIds = ["9"],
                    Cage = null
                }
            ],
            CatDays.NineDoubleCageShared1 =>
            [
                new CreateCatDayDto
                {
                    RentedCageTypeId = new RentedCageGroup("120;120;120;Double;RentedWithOneOtherCat"),
                    ExhibitorsCage = null,
                    ExhibitionDayId = exhibitionDaysId.First(),
                    GroupsIds = ["9"],
                    Cage = null
                }
            ],
            CatDays.Single =>
            [
                new CreateCatDayDto
                {
                    RentedCageTypeId = new RentedCageGroup("120;120;120;Single;RentedWithZeroOtherCats"),
                    ExhibitorsCage = null,
                    ExhibitionDayId = exhibitionDaysId.First(),
                    GroupsIds = ["9"],
                    Cage = null
                }
            ],
            CatDays.NineDoubleCageShared2 =>
            [
                new CreateCatDayDto
                {
                    RentedCageTypeId = new RentedCageGroup("120;120;120;Double;RentedWithTwoOtherCats"),
                    ExhibitorsCage = null,
                    ExhibitionDayId = exhibitionDaysId.First(),
                    GroupsIds = ["9"],
                    Cage = null
                }
            ],
            CatDays.CatDays2 =>
            [
                new CreateCatDayDto
                {
                    RentedCageTypeId = new RentedCageGroup("120;120;120;Double;RentedWithZeroOtherCats"),
                    ExhibitorsCage = null,
                    ExhibitionDayId = exhibitionDaysId.First(),
                    GroupsIds = ["16"],
                    Cage = null
                }
            ],
            CatDays.BothDays =>
            [
                new CreateCatDayDto
                {
                    RentedCageTypeId = new RentedCageGroup("120;120;120;Double;RentedWithZeroOtherCats"),
                    ExhibitorsCage = null,
                    ExhibitionDayId = exhibitionDaysId[0],
                    GroupsIds = ["9"],
                    Cage = null
                },
                new CreateCatDayDto
                {
                    RentedCageTypeId = new RentedCageGroup("120;120;120;Double;RentedWithZeroOtherCats"),
                    ExhibitorsCage = null,
                    ExhibitionDayId = exhibitionDaysId[1],
                    GroupsIds = ["9"],
                    Cage = null
                }
            ],
            _ => throw new ArgumentOutOfRangeException(nameof(catDay), catDay, null)
        };
    }
}
