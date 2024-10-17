#region

using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
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

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.CatRegistrations.Commands.
    CreateCatRegistration;

#region

using static Testing;

#endregion

public class CreateCatRegistrationAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    private readonly TestData _testData = new();

    [Test]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldCreateCatRegistrationWithLitter(RunAsSpecificUser runAsSpecificUser)
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
            GroupsIds = ["1", "2"],
            ExhibitionId = exhibition1Id,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = exhibitionDays.Select(x => x.Id).ToList(),
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1Id
        });
        await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id });
        string sabrinaId = await RunAsSabrinaAsync();
        int sabrinaExhibitorId =
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
                    sabrinaExhibitorId, advertisements.First().Id)
            };

        Result<int> registrationToExhibitionId = await SendAsync(createRegistrationToExhibitionCommand);
        await RunAsExecutor(runAsSpecificUser);
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = null,
                Litter = _testData.GetLitterDto(TestData.Litters.Litter1, TestData.Breeders.Breeder1,
                    TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Note = null,
                CatDays =
                [
                    new CreateCatDayDto
                    {
                        RentedCageTypeId = null,
                        ExhibitorsCage = null,
                        ExhibitionDayId = exhibitionDays.First().Id,
                        GroupsIds = ["1"],
                        Cage = null
                    }
                ]
            }
        };

        // Act
        Func<Task> act = async () => await SendAsync(command);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    public async Task ShouldFailCreateCatRegistrationWithLitter(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
        await RunAsExecutor(runAsSpecificUser);
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = null,
                Litter = _testData.GetLitterDto(TestData.Litters.Litter1, TestData.Breeders.Breeder1,
                    TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Note = null,
                CatDays =
                [
                    new CreateCatDayDto
                    {
                        RentedCageTypeId = null,
                        ExhibitorsCage = null,
                        ExhibitionDayId = exhibitionDays.First().Id,
                        GroupsIds = ["1"],
                        Cage = null
                    }
                ]
            }
        };

        // Act
        Func<Task> act = async () => await SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
