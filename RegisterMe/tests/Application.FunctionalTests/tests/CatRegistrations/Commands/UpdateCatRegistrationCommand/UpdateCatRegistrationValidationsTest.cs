#region

using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Domain.Common;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.CatRegistrations.Commands.
    UpdateCatRegistrationCommand;

public class ShouldUpdateCatRegistrationValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    private readonly TestData _testData = new();

    [Test]
    [TestCase(TestData.CatDays.NineDoubleCage, null, null, null)]
    [TestCase(TestData.CatDays.NineDoubleCage, TestData.ExhibitedCats.ExhibitedCat1, TestData.Litters.Litter2, null)]
    public async Task ShouldFailUpdateCatRegistration(TestData.CatDays catDays, TestData.ExhibitedCats? exhibitedCats,
        TestData.Litters? litters,
        string? note)
    {
        // Arrange
        CreateExhibitedCatDto? exhibitedCat = exhibitedCats.HasValue
            ? _testData.GetExhibitedCatDto(exhibitedCats.Value, TestData.Breeders.Breeder1, TestData.Fathers.Father1,
                TestData.Mothers.Mother1)
            : null;
        CreateLitterDto? litter = litters.HasValue
            ? _testData.GetLitterDto(litters.Value, TestData.Breeders.Breeder1, TestData.Fathers.Father1,
                TestData.Mothers.Mother1)
            : null;
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
        List<CreateCatDayDto> catDaysDto = _testData.GetCatDayDto(catDays, exhibitionDays.Select(x => x.Id).ToList());
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat =
                    _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
                        TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Litter = null,
                Note = note,
                CatDays = _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage,
                    exhibitionDays.Select(x => x.Id).ToList())
            }
        };
        Result<int> catRegistrationId = await SendAsync(command);
        catRegistrationId.IsSuccess.Should().BeTrue();

        Application.CatRegistrations.Commands.UpdateCatRegistrationCommand.UpdateCatRegistrationCommand updateCommand =
            new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Id = catRegistrationId.Value,
                    ExhibitedCat = exhibitedCat,
                    Litter = litter,
                    Note = note,
                    CatDays = catDaysDto
                }
            };

        // Act
        Result<int> updatedCatRegistrationId = await SendAsync(updateCommand);

        // Assert
        updatedCatRegistrationId.IsSuccess.Should().BeFalse();
    }


    [Test]
    [TestCase(TestData.CatDays.NineDoubleCage, TestData.ExhibitedCats.ExhibitedCat1, null, null)]
    [TestCase(TestData.CatDays.NineDoubleCage, TestData.ExhibitedCats.ExhibitedCat2, null, null)]
    [TestCase(TestData.CatDays.CatDays2, null, TestData.Litters.Litter1, null)]
    [TestCase(TestData.CatDays.CatDays2, null, TestData.Litters.Litter2, null)]
    [TestCase(TestData.CatDays.NineDoubleCage, TestData.ExhibitedCats.ExhibitedCat1, null, "t")]
    [TestCase(TestData.CatDays.NineDoubleCage, TestData.ExhibitedCats.ExhibitedCat2, null, "d")]
    [TestCase(TestData.CatDays.CatDays2, null, TestData.Litters.Litter1, "df")]
    [TestCase(TestData.CatDays.CatDays2, null, TestData.Litters.Litter2, "df")]
    public async Task ShouldUpdateCatRegistration(TestData.CatDays catDays, TestData.ExhibitedCats? exhibitedCats,
        TestData.Litters? litters,
        string? note)
    {
        // Arrange
        CreateExhibitedCatDto? exhibitedCat = exhibitedCats.HasValue
            ? _testData.GetExhibitedCatDto(exhibitedCats.Value, TestData.Breeders.Breeder1, TestData.Fathers.Father1,
                TestData.Mothers.Mother1)
            : null;
        CreateLitterDto? litter = litters.HasValue
            ? _testData.GetLitterDto(litters.Value, TestData.Breeders.Breeder1, TestData.Fathers.Father1,
                TestData.Mothers.Mother1)
            : null;
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
        List<CreateCatDayDto> catDaysDto = _testData.GetCatDayDto(catDays, exhibitionDays.Select(x => x.Id).ToList());
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat =
                    _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
                        TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Litter = null,
                Note = note,
                CatDays = _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage,
                    exhibitionDays.Select(x => x.Id).ToList())
            }
        };


        Result<int> catRegistrationId = await SendAsync(command);
        catRegistrationId.IsSuccess.Should().BeTrue();

        Application.CatRegistrations.Commands.UpdateCatRegistrationCommand.UpdateCatRegistrationCommand updateCommand =
            new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Id = catRegistrationId.Value,
                    ExhibitedCat = exhibitedCat,
                    Litter = litter,
                    Note = note,
                    CatDays = catDaysDto
                }
            };

        // Act
        Result<int> updatedCatRegistrationId = await SendAsync(updateCommand);

        // Assert
        updatedCatRegistrationId.IsSuccess.Should().BeTrue();
    }
}
