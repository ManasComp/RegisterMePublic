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
using RegisterMe.Application.RegistrationToExhibition.Commands.DeleteRegistrationToExhibition;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.
    DeleteRegistrationToExhibition;

#region

using static Testing;

#endregion

public class DeleteRegistrationToExhibitionAuthorizationTest(DatabaseTypes databaseType)
    : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldDeleteRegistrationToExhibition(RunAsSpecificUser runAsSpecificUser)
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
        int sabrinaExhibitor =
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
                    sabrinaExhibitor, advertisements.First().Id)
            };
        Result<int> registrationToExhibitionId = await SendAsync(createRegistrationToExhibitionCommand);

        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () =>
            await SendAsync(new DeleteRegistrationToExhibitionCommand
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value
            });

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsVojta)]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    public async Task ShouldFailDeleteRegistrationToExhibition(RunAsSpecificUser runAsSpecificUser)
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
        int sabrinaExhibitor =
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
                    sabrinaExhibitor, advertisements.First().Id)
            };
        Result<int> registrationToExhibitionId = await SendAsync(createRegistrationToExhibitionCommand);
        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () =>
            await SendAsync(new DeleteRegistrationToExhibitionCommand
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value
            });

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
