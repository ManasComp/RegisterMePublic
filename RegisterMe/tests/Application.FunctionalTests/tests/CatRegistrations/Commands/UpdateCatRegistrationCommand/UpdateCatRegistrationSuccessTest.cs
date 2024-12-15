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
    UpdateCatRegistrationCommand;

public class UpdateCatRegistrationSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    private readonly TestData _testData = new();

    [Test]
    public async Task ShouldFailCreateCatRegistrationForEmptyLitterAndEMptyCat()
    {
        // Arrange
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
        CreateExhibitedCatDto createxhibitedCat = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
            TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1);
        List<CreateCatDayDto> catDaysDto =
            _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage, exhibitionDays.Select(x => x.Id).ToList());
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = createxhibitedCat,
                Litter = null,
                Note = null,
                CatDays = catDaysDto
            }
        };
        Result<int> catRegistrationId = await SendAsync(command);
        catRegistrationId.IsSuccess.Should().BeTrue();

        List<CreateCatDayDto> updateCatDays =
            _testData.GetCatDayDto(TestData.CatDays.CatDays2, exhibitionDays.Select(x => x.Id).ToList());
        Application.CatRegistrations.Commands.UpdateCatRegistrationCommand.UpdateCatRegistrationCommand
            updateCatRegistrationCommand = new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Id = catRegistrationId.Value,
                    ExhibitedCat = null,
                    CatDays = updateCatDays,
                    Note = "Updated",
                    Litter = null
                }
            };

        // Act
        Result<int> updatedCatRegistrationId = await SendAsync(updateCatRegistrationCommand);

        // Assert
        updatedCatRegistrationId.IsSuccess.Should().BeFalse();
    }

    [Test]
    public async Task ShouldUpdateCatRegistrationFromCatToCat()
    {
        // Arrange
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
        CreateExhibitedCatDto creaeCatRegistration = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
            TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1);
        List<CreateCatDayDto> catDaysDto =
            _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage, exhibitionDays.Select(x => x.Id).ToList());
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = creaeCatRegistration,
                Litter = null,
                Note = null,
                CatDays = catDaysDto
            }
        };
        Result<int> catRegistrationId = await SendAsync(command);
        catRegistrationId.IsSuccess.Should().BeTrue();

        CatRegistrationDto catRegistration =
            await SendAsync(new GetCatRegistrationByIdQuery { Id = catRegistrationId.Value });
        catRegistration.Should().NotBeNull();
        CompareUtils.Equals(command.CatRegistration, catRegistration).Should().BeTrue();

        CreateExhibitedCatDto updateCrreateCatRegistration = _testData.GetExhibitedCatDto(
            TestData.ExhibitedCats.ExhibitedCat1, TestData.Breeders.Breeder1, TestData.Fathers.Father1,
            TestData.Mothers.Mother1);
        List<CreateCatDayDto> updateCatDays =
            _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage, exhibitionDays.Select(x => x.Id).ToList());
        Application.CatRegistrations.Commands.UpdateCatRegistrationCommand.UpdateCatRegistrationCommand
            updateCatRegistrationCommand = new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Id = catRegistrationId.Value,
                    ExhibitedCat = updateCrreateCatRegistration,
                    CatDays = updateCatDays,
                    Note = "Updated",
                    Litter = null
                }
            };

        // Act
        Result<int> updatedCatRegistrationId = await SendAsync(updateCatRegistrationCommand);

        // Assert
        updatedCatRegistrationId.IsSuccess.Should().BeTrue();
        CatRegistrationDto updatedCatRegistration =
            await SendAsync(new GetCatRegistrationByIdQuery { Id = updatedCatRegistrationId.Value });
        CompareUtils.Equals(updateCatRegistrationCommand.CatRegistration, updatedCatRegistration).Should().BeTrue();
    }

    [Test]
    public async Task ShouldUpdateCatRegistrationFromLitterToExhibitedCat()
    {
        // Arrange
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
        CreateLitterDto createLitterDto = _testData.GetLitterDto(TestData.Litters.Litter1, TestData.Breeders.Breeder1,
            TestData.Fathers.Father1, TestData.Mothers.Mother1);
        List<CreateCatDayDto> catDaysDto =
            _testData.GetCatDayDto(TestData.CatDays.CatDays2, exhibitionDays.Select(x => x.Id).ToList());
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = null,
                Litter = createLitterDto,
                Note = null,
                CatDays = catDaysDto
            }
        };
        Result<int> catRegistrationId = await SendAsync(command);
        catRegistrationId.IsSuccess.Should().BeTrue();

        CatRegistrationDto catRegistration =
            await SendAsync(new GetCatRegistrationByIdQuery { Id = catRegistrationId.Value });
        catRegistration.Should().NotBeNull();
        CompareUtils.Equals(command.CatRegistration, catRegistration).Should().BeTrue();

        CreateExhibitedCatDto updateCrreateCatRegistration = _testData.GetExhibitedCatDto(
            TestData.ExhibitedCats.ExhibitedCat1, TestData.Breeders.Breeder1, TestData.Fathers.Father1,
            TestData.Mothers.Mother1);
        List<CreateCatDayDto> updateCatDays =
            _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage, exhibitionDays.Select(x => x.Id).ToList());
        Application.CatRegistrations.Commands.UpdateCatRegistrationCommand.UpdateCatRegistrationCommand
            updateCatRegistrationCommand = new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Id = catRegistrationId.Value,
                    ExhibitedCat = updateCrreateCatRegistration,
                    CatDays = updateCatDays,
                    Note = "Updated",
                    Litter = null
                }
            };

        // Act
        Result<int> updatedCatRegistrationId = await SendAsync(updateCatRegistrationCommand);

        // Assert
        updatedCatRegistrationId.IsSuccess.Should().BeTrue();
        CatRegistrationDto updatedCatRegistration =
            await SendAsync(new GetCatRegistrationByIdQuery { Id = updatedCatRegistrationId.Value });
        CompareUtils.Equals(updateCatRegistrationCommand.CatRegistration, updatedCatRegistration).Should().BeTrue();
    }

    [Test]
    public async Task ShouldUpdateCatRegistrationFromExhibitedCatToLittter()
    {
        // Arrange
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
        CreateExhibitedCatDto createExhibitedCat = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
            TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1);
        List<CreateCatDayDto> catDaysDto =
            _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage, exhibitionDays.Select(x => x.Id).ToList());
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = createExhibitedCat,
                Litter = null,
                Note = null,
                CatDays = catDaysDto
            }
        };
        Result<int> catRegistrationId = await SendAsync(command);
        catRegistrationId.IsSuccess.Should().BeTrue();

        CatRegistrationDto catRegistration =
            await SendAsync(new GetCatRegistrationByIdQuery { Id = catRegistrationId.Value });
        catRegistration.Should().NotBeNull();
        CompareUtils.Equals(command.CatRegistration, catRegistration).Should().BeTrue();

        CreateLitterDto updateLitterDto = _testData.GetLitterDto(TestData.Litters.Litter1, TestData.Breeders.Breeder1,
            TestData.Fathers.Father1, TestData.Mothers.Mother1);
        List<CreateCatDayDto> updateCatDays =
            _testData.GetCatDayDto(TestData.CatDays.CatDays2, exhibitionDays.Select(x => x.Id).ToList());
        Application.CatRegistrations.Commands.UpdateCatRegistrationCommand.UpdateCatRegistrationCommand
            updateCatRegistrationCommand = new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Id = catRegistrationId.Value,
                    ExhibitedCat = null,
                    CatDays = updateCatDays,
                    Note = "Updated",
                    Litter = updateLitterDto
                }
            };

        // Act
        Result<int> updatedCatRegistrationId = await SendAsync(updateCatRegistrationCommand);

        // Assert
        updatedCatRegistrationId.IsSuccess.Should().BeTrue();
        CatRegistrationDto updatedCatRegistration =
            await SendAsync(new GetCatRegistrationByIdQuery { Id = updatedCatRegistrationId.Value });
        CompareUtils.Equals(updateCatRegistrationCommand.CatRegistration, updatedCatRegistration).Should().BeTrue();
    }

    [Test]
    public async Task ShouldUpdateCatRegistrationFromLitterToLittter()
    {
        // Arrange
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
        CreateLitterDto litter = _testData.GetLitterDto(TestData.Litters.Litter1, TestData.Breeders.Breeder1,
            TestData.Fathers.Father1, TestData.Mothers.Mother1);
        List<CreateCatDayDto> catDaysDto =
            _testData.GetCatDayDto(TestData.CatDays.CatDays2, exhibitionDays.Select(x => x.Id).ToList());
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = null,
                Litter = litter,
                Note = null,
                CatDays = catDaysDto
            }
        };
        Result<int> catRegistrationId = await SendAsync(command);
        catRegistrationId.IsSuccess.Should().BeTrue();

        CatRegistrationDto catRegistration =
            await SendAsync(new GetCatRegistrationByIdQuery { Id = catRegistrationId.Value });
        catRegistration.Should().NotBeNull();
        CompareUtils.Equals(command.CatRegistration, catRegistration).Should().BeTrue();

        CreateLitterDto updateLitterDto = _testData.GetLitterDto(TestData.Litters.Litter2, TestData.Breeders.Breeder1,
            TestData.Fathers.Father1, TestData.Mothers.Mother1);
        List<CreateCatDayDto> updateCatDays =
            _testData.GetCatDayDto(TestData.CatDays.CatDays2, exhibitionDays.Select(x => x.Id).ToList());
        Application.CatRegistrations.Commands.UpdateCatRegistrationCommand.UpdateCatRegistrationCommand
            updateCatRegistrationCommand = new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Id = catRegistrationId.Value,
                    ExhibitedCat = null,
                    CatDays = updateCatDays,
                    Note = "Updated",
                    Litter = updateLitterDto
                }
            };

        // Act
        Result<int> updatedCatRegistrationId = await SendAsync(updateCatRegistrationCommand);

        // Assert
        updatedCatRegistrationId.IsSuccess.Should().BeTrue();
        CatRegistrationDto updatedCatRegistration =
            await SendAsync(new GetCatRegistrationByIdQuery { Id = updatedCatRegistrationId.Value });
        CompareUtils.Equals(updateCatRegistrationCommand.CatRegistration, updatedCatRegistration).Should().BeTrue();
    }
}
