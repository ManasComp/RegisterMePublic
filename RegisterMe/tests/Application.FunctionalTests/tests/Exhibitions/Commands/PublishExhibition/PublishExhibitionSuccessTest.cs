#region

using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Commands.PublishExhibition;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionById;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.PublishExhibition;

public class PublishExhibitionsSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldPublishExhibition(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user1 = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user1)
        })).Value;
        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });

        Result<int> exhibition1Id = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        });

        await RunAsExecutor(runAsSpecificUser);
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibition1Id.Value });
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["1", "2"],
            ExhibitionId = exhibition1Id.Value,
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
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1Id.Value
        });

        // Act
        Result result = await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id.Value });

        // Assert
        result.IsSuccess.Should().BeTrue();
        BriefExhibitionDto isConfirmed =
            await SendAsync(new GetExhibitionByIdQuery { ExhibitionId = exhibition1Id.Value });
        isConfirmed.IsPublished.Should().BeTrue();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldFailPublishExhibitionWithoutPrices(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user1 = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user1)
        })).Value;
        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });

        Result<int> exhibition1Id = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        });

        await RunAsExecutor(runAsSpecificUser);

        // Act
        Result result = await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id.Value });

        // Assert
        result.IsSuccess.Should().BeFalse();
        BriefExhibitionDto isConfirmed =
            await SendAsync(new GetExhibitionByIdQuery { ExhibitionId = exhibition1Id.Value });
        isConfirmed.IsPublished.Should().BeFalse();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldFailPublishExhibitionInUnconfirmedOrganization(
        RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user1 = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user1)
        })).Value;

        Result<int> exhibition1Id = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        });

        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["1", "2"],
            ExhibitionId = exhibition1Id.Value,
            PriceDays = [new PriceDays { ExhibitionDayIds = [1, 2], Price = new MultiCurrencyPrice(100, 3) }]
        });

        await RunAsExecutor(runAsSpecificUser);

        // Act
        Result result = await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id.Value });

        // Assert
        result.IsSuccess.Should().BeFalse();
        BriefExhibitionDto isConfirmed =
            await SendAsync(new GetExhibitionByIdQuery { ExhibitionId = exhibition1Id.Value });
        isConfirmed.IsPublished.Should().BeFalse();
    }
}
