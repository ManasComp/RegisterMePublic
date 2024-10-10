#region

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
using RegisterMe.Application.RegistrationToExhibition.Commands.ChangeAdvertisement;
using RegisterMe.Application.RegistrationToExhibition.Commands.CreateRegistrationToExhibition;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.ChangeAdvertisement;

#region

using static Testing;

#endregion

public class ChangeAdvertisementAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldCreateRegistrationToExhibition(RunAsSpecificUser runAsSpecificUser)
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
        UpsertAdvertisementDto nextAdvertisement = AdvertisementDataGenerator.GetAdvertisementDto2();
        nextAdvertisement.IsDefault = false;
        Result<int> advertisementId = await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = nextAdvertisement, ExhibitionId = exhibition1Id
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

        Result<int> registrationToExhibitionId = await SendAsync(
            new CreateRegistrationToExhibitionCommand
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            });

        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> changeAdvertisement = async () => await SendAsync(new ChangeAdvertisementsCommand
        {
            AdvertisementId = advertisementId.Value, RegistrationToExhibitionId = registrationToExhibitionId.Value
        });

        // Assert
        await changeAdvertisement.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsVojta)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    public async Task ShouldFailCreateRegistrationToExhibition(RunAsSpecificUser runAsSpecificUser)
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
        UpsertAdvertisementDto nextAdverttisement = AdvertisementDataGenerator.GetAdvertisementDto2();
        nextAdverttisement.IsDefault = false;
        Result<int> advertisementId = await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = nextAdverttisement, ExhibitionId = exhibition1Id
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

        // Act
        Result<int> registrationToExhibitionId = await SendAsync(
            new CreateRegistrationToExhibitionCommand
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            });

        await RunAsExecutor(runAsSpecificUser);
        Func<Task> changeAdvertisement = async () => await SendAsync(new ChangeAdvertisementsCommand
        {
            AdvertisementId = advertisementId.Value, RegistrationToExhibitionId = registrationToExhibitionId.Value
        });

        //assert

        await changeAdvertisement.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
