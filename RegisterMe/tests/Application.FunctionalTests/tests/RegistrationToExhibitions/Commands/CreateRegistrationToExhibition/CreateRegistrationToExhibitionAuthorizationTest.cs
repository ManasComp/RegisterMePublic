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
using RegisterMe.Application.ValueTypes;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.
    CreateRegistrationToExhibition;

#region

using static Testing;

#endregion

public class CreateRegistrationToExhibitionAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    public async Task ShouldCreateRegistrationToExhibitionForDefaultUser(RunAsSpecificUser runAsSpecificUser)
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
        await RunAsExecutor(runAsSpecificUser);

        // Act
        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            };
        Func<Task> act = async () => await SendAsync(createRegistrationToExhibitionCommand);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    public async Task ShouldCreateRegistrationToExhibitionForGivenUser(RunAsSpecificUser runAsSpecificUser)
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
        string? userId = await RunAsExecutor(runAsSpecificUser);
        int createExhibitorCommand =
            (await SendAsync(new CreateExhibitorCommand
            {
                UserId = userId ?? user, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
            })).Value;
        List<AdvertisementDto> advertisements =
            await SendAsync(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = exhibition1Id });

        // Act
        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            };
        Func<Task> act = async () => await SendAsync(createRegistrationToExhibitionCommand);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    public async Task ShouldFailCreateRegistrationToExhibition(RunAsSpecificUser runAsSpecificUser)
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
        await RunAsExecutor(runAsSpecificUser);

        // Act
        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            };
        Func<Task> act = async () => await SendAsync(createRegistrationToExhibitionCommand);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
