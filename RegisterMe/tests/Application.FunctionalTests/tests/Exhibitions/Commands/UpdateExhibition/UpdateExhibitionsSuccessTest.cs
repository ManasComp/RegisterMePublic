#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Commands.PublishExhibition;
using RegisterMe.Application.Exhibitions.Commands.UpdateExhibition;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionById;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.UpdateExhibition;

#region

using static Testing;

#endregion

public class UpdateExhibitionsSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldUpdateExhibition()
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

        await RunAsOndrejAsync();

        UpdateExhibitionDto updatedExhibition =
            ExhibitionDataGenerator.UpdatedExhibition(ExhibitionDataGenerator.Exhibition2(organization1),
                exhibition1Id.Value);

        // Act
        Result result = await SendAsync(new UpdateExhibitionCommand { UpdateExhibitionDto = updatedExhibition });

        // Assert
        BriefExhibitionDto getExhibition =
            await SendAsync(new GetExhibitionByIdQuery { ExhibitionId = exhibition1Id.Value });
        result.IsSuccess.Should().BeTrue();
        CompareUtils.Equals(updatedExhibition, getExhibition).Should().BeTrue();
    }

    [Test]
    public async Task ShouldFailUpdatePublishedExhibition()
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

        await RunAsOndrejAsync();

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
        (await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id.Value })).IsSuccess.Should()
            .BeTrue();

        UpdateExhibitionDto updatedExhibition =
            ExhibitionDataGenerator.UpdatedExhibition(ExhibitionDataGenerator.Exhibition2(organization1),
                exhibition1Id.Value);

        // Assert
        Func<Task> act = async () =>
            await SendAsync(new UpdateExhibitionCommand { UpdateExhibitionDto = updatedExhibition });

        // Assert
        BriefExhibitionDto getExhibition =
            await SendAsync(new GetExhibitionByIdQuery { ExhibitionId = exhibition1Id.Value });

        await act.Should().ThrowAsync<ForbiddenAccessException>();
        CompareUtils.Equals(ExhibitionDataGenerator.Exhibition1(organization1), getExhibition).Should().BeTrue();
    }
}
