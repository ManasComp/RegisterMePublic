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
using RegisterMe.Application.RegistrationToExhibition.Commands.CreateRegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.ValueTypes;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.
    CreateRegistrationToExhibition;

#region

using static Testing;

#endregion

public class CreateRegistrationToExhibitionValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(null, null, null)]
    public async Task ShouldCreateRegistrationToExhibition(int? exhibitionId, int? exhibitorId,
        int? advertisementId)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
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
        int createExhibitorCommand =
            (await SendAsync(new CreateExhibitorCommand
            {
                UserId = user, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
            })).Value;
        List<AdvertisementDto> advertisements =
            await SendAsync(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = exhibition1Id });

        // Act
        RegistrationToExhibitionDataGenerator.Normal(
            exhibition1Id,
            createExhibitorCommand, advertisements.First().Id);
        int localAdvertisementId = advertisementId ?? advertisements.First().Id;
        int localExhibitorId = exhibitorId ?? createExhibitorCommand;
        int localExhibitionId = exhibitionId ?? exhibition1Id;
        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = new CreateRegistrationToExhibitionDto
                {
                    AdvertisementId = localAdvertisementId,
                    ExhibitionId = localExhibitionId,
                    ExhibitorId = localExhibitorId
                }
            };
        Func<Task> act = async () => await SendAsync(createRegistrationToExhibitionCommand);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(0, null, null)]
    [TestCase(-1, null, null)]
    [TestCase(null, 0, null)]
    [TestCase(null, -1, null)]
    [TestCase(null, null, 0)]
    [TestCase(null, null, -1)]
    public async Task ShouldFailCreateRegistrationToExhibition(int? exhibitionId, int? exhibitorId,
        int? advertisementId)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
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
        int createExhibitorCommand =
            (await SendAsync(new CreateExhibitorCommand
            {
                UserId = user, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
            })).Value;
        List<AdvertisementDto> advertisements =
            await SendAsync(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = exhibition1Id });

        // Act
        int localAdvertisementId = advertisementId ?? advertisements.First().Id;
        int localExhibitorId = exhibitorId ?? createExhibitorCommand;
        int localExhibitionId = exhibitionId ?? exhibition1Id;
        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = new CreateRegistrationToExhibitionDto
                {
                    AdvertisementId = localAdvertisementId,
                    ExhibitionId = localExhibitionId,
                    ExhibitorId = localExhibitorId
                }
            };
        Func<Task> act = async () => await SendAsync(createRegistrationToExhibitionCommand);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
