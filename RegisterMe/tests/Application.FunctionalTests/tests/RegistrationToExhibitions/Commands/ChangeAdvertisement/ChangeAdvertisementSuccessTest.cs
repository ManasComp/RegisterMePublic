#region

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
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.ChangeAdvertisement;

public class ChangeAdvertisementSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldChangeAdvertisement()
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
        UpsertAdvertisementDto nextAdvertisemnt = AdvertisementDataGenerator.GetAdvertisementDto2();
        nextAdvertisemnt.IsDefault = false;
        Result<int> advertisementId = await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = nextAdvertisemnt, ExhibitionId = exhibition1Id
        });
        await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id });
        int createExhibitorCommand =
            (await SendAsync(new CreateExhibitorCommand
            {
                UserId = ondrejId, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
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

        // Act
        Result result = await SendAsync(new ChangeAdvertisementsCommand
        {
            AdvertisementId = advertisementId.Value, RegistrationToExhibitionId = registrationToExhibitionId.Value
        });

        // Assert
        result.IsSuccess.Should().BeTrue();
        RegistrationToExhibitionDto registrationToExhibition =
            await SendAsync(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value
            });
        registrationToExhibition.AdvertisementId.Should().Be(advertisementId.Value);
    }
}
