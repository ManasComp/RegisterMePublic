#region

using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Commands.CreateRentedCage;
using RegisterMe.Application.Exhibitions.Commands.PublishExhibition;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetAdvertisementsByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.RegistrationToExhibition.Commands.CreateRegistrationToExhibition;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.CatRegistrations.Commands.
    UpdateCatRegistrationCommand;

#region

using static Testing;

#endregion

public class UpdateCatRegistrationAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    private readonly TestData _testData = new();

    [Test]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldUpdateRegistration(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string ondrejId = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(ondrejId)
        })).Value;

        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });
        await RunAsOndrejAsync();

        int exhibition1Id = (await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        })).Value;
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibition1Id });
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "16"],
            ExhibitionId = exhibition1Id,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = [exhibitionDays.Select(x => x.Id).ToList().First()],
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1Id
        });
        await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 5,
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList(),
                Height = 120,
                Width = 120,
                Length = 120,
                RentedCageTypes = [RentedType.Double, RentedType.Single]
            }
        });
        await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id });

        string sabrinaId = await RunAsSabrinaAsync();
        int createExhibitorCommand =
            (await SendAsync(new CreateExhibitorCommand
            {
                UserId = sabrinaId, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
            })).Value;
        List<AdvertisementDto> advertisements =
            await SendAsync(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = exhibition1Id });

        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            };

        Result<int> registrationToExhibitionId = await SendAsync(createRegistrationToExhibitionCommand);

        List<CreateCatDayDto> catDays =
            _testData.GetCatDayDto(TestData.CatDays.CatDays2, exhibitionDays.Select(x => x.Id).ToList());

        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = null,
                Litter = _testData.GetLitterDto(TestData.Litters.Litter1, TestData.Breeders.Breeder1,
                    TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Note = null,
                CatDays = catDays
            }
        };
        Result<int> catRegistrationId = await SendAsync(command);
        await RunAsExecutor(runAsSpecificUser);
        List<CreateCatDayDto> updateCatDays =
            _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage, exhibitionDays.Select(x => x.Id).ToList());
        CreateExhibitedCatDto updateCreateCatRegistration = _testData.GetExhibitedCatDto(
            TestData.ExhibitedCats.ExhibitedCat1, TestData.Breeders.Breeder1, TestData.Fathers.Father1,
            TestData.Mothers.Mother1);
        Application.CatRegistrations.Commands.UpdateCatRegistrationCommand.UpdateCatRegistrationCommand
            updateCatRegistrationCommand = new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Id = catRegistrationId.Value,
                    ExhibitedCat = updateCreateCatRegistration,
                    CatDays = updateCatDays,
                    Note = "Updated",
                    Litter = null
                }
            };

        // Act
        Func<Task> updatedCatRegistrationId = async () => await SendAsync(updateCatRegistrationCommand);

        // Assert
        await updatedCatRegistrationId.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsVojta)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    public async Task ShouldFailUpdateCatRegistration(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string ondrejId = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(ondrejId)
        })).Value;

        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });
        await RunAsOndrejAsync();

        int exhibition1Id = (await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        })).Value;
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibition1Id });
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "16"],
            ExhibitionId = exhibition1Id,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = [exhibitionDays.Select(x => x.Id).ToList().First()],
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1Id
        });
        await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 5,
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList(),
                Height = 120,
                Width = 120,
                Length = 120,
                RentedCageTypes = [RentedType.Double, RentedType.Single]
            }
        });
        await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id });

        string sabrinaId = await RunAsSabrinaAsync();
        int createExhibitorCommand =
            (await SendAsync(new CreateExhibitorCommand
            {
                UserId = sabrinaId, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
            })).Value;
        List<AdvertisementDto> advertisements =
            await SendAsync(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = exhibition1Id });

        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            };

        Result<int> registrationToExhibitionId = await SendAsync(createRegistrationToExhibitionCommand);

        List<CreateCatDayDto> catDays =
            _testData.GetCatDayDto(TestData.CatDays.CatDays2, exhibitionDays.Select(x => x.Id).ToList());

        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = null,
                Litter = _testData.GetLitterDto(TestData.Litters.Litter1, TestData.Breeders.Breeder1,
                    TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Note = null,
                CatDays = catDays
            }
        };
        Result<int> catRegistrationId = await SendAsync(command);
        await RunAsExecutor(runAsSpecificUser);
        List<CreateCatDayDto> updateCatDays =
            _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage, exhibitionDays.Select(x => x.Id).ToList());
        CreateExhibitedCatDto updateCrreateCatRegistration = _testData.GetExhibitedCatDto(
            TestData.ExhibitedCats.ExhibitedCat1, TestData.Breeders.Breeder1, TestData.Fathers.Father1,
            TestData.Mothers.Mother1);
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
        Func<Task> updatedCatRegistrationId = async () => await SendAsync(updateCatRegistrationCommand);

        // Assert
        await updatedCatRegistrationId.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
