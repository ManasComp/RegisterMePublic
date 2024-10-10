#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Commands.UpdatePriceGroup;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.UpdatePriceGroup;

#region

using static Testing;

#endregion

public class UpdatePricesAuthorizationTes(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    public async Task ShouldFailUpdatePrices(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;

        Result<int> exhibitionId = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organizationId)
        });

        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId.Value });
        Result<string> ids = await SendAsync(new CreatePriceGroupCommand
        {
            ExhibitionId = exhibitionId.Value,
            GroupsIds = ["1", "2"],
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = [..exhibitionDays.Select(x => x.Id)], Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () => await SendAsync(new UpdatePriceGroupCommand
        {
            OriginalPricesId = ids.Value,
            GroupsIds = ["1", "2"],
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = [..exhibitionDays.Select(x => x.Id)], Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    public async Task ShouldUpdatePrices(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;

        Result<int> exhibitionId = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organizationId)
        });

        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId.Value });

        Result<string> ids = await SendAsync(new CreatePriceGroupCommand
        {
            ExhibitionId = exhibitionId.Value,
            GroupsIds = ["1", "2"],
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = [..exhibitionDays.Select(x => x.Id)], Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () => await SendAsync(new UpdatePriceGroupCommand
        {
            OriginalPricesId = ids.Value,
            GroupsIds = ["1", "2"],
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = [..exhibitionDays.Select(x => x.Id)], Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });

        // Assert
        await act.Should().NotThrowAsync();
    }
}
