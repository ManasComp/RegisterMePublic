#region

using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.CatRegistrations.Queries.GetCatRegistrationById;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Domain.Common;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.CatRegistrations.Commands.
    CreateCatRegistration;

public class ShouldCreateCatRegistrationValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    private readonly TestData _testData = new();

    [Test]
    [TestCase(TestData.CatDays.NineDoubleCage, null, null, null)]
    [TestCase(TestData.CatDays.NineDoubleCage, TestData.ExhibitedCats.ExhibitedCat1, TestData.Litters.Litter2, null)]
    public async Task ShouldFailCreateCatRegistration(TestData.CatDays catDays, TestData.ExhibitedCats? exhibitedCats,
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
                ExhibitedCat = exhibitedCat,
                Litter = litter,
                Note = note,
                CatDays = catDaysDto
            }
        };

        // Act
        Result<int> catRegistrationId = await SendAsync(command);

        // Assert
        catRegistrationId.IsSuccess.Should().BeFalse();
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
    public async Task ShouldCreateCatRegistration(TestData.CatDays catDays, TestData.ExhibitedCats? exhibitedCats,
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
                ExhibitedCat = exhibitedCat,
                Litter = litter,
                Note = note,
                CatDays = catDaysDto
            }
        };

        // Act
        Result<int> catRegistrationId = await SendAsync(command);

        // Assert
        catRegistrationId.IsSuccess.Should().BeTrue();

        CatRegistrationDto catRegistration =
            await SendAsync(new GetCatRegistrationByIdQuery { Id = catRegistrationId.Value });
        catRegistration.Should().NotBeNull();
        CompareUtils.Equals(command.CatRegistration, catRegistration).Should().BeTrue();
    }
}
