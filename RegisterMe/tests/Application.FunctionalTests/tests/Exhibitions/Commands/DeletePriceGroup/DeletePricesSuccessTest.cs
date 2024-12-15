#region

using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Commands.DeletePriceGroup;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionGroupById;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.DeletePriceGroup;

#region

using static Testing;

#endregion

public class DeletePricesSuccessTeste(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldDeletePrices()
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

        List<PriceDays> priceDays =
        [
            new()
            {
                ExhibitionDayIds = exhibitionDays.Select(x => x.Id).ToList(), Price = new MultiCurrencyPrice(100, 3)
            }
        ];
        CreatePriceGroupCommand createPriceGroupCommand = new()
        {
            GroupsIds = ["4", "5"], ExhibitionId = exhibitionId.Value, PriceDays = priceDays
        };

        Result<string> pricesId = await SendAsync(createPriceGroupCommand);
        await SendAsync(new GetExhibitionGroupByIdQuery { GroupsId = pricesId.Value });

        // Act
        Result result = await SendAsync(new DeletePriceGroupCommand { PriceIds = pricesId.Value });

        // Assert
        result.IsSuccess.Should().BeTrue();
        Result<BigPriceDto> value =
            await SendAsync(new GetExhibitionGroupByIdQuery { GroupsId = pricesId.Value });

        value.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task ShouldFailDeleteCreatePricesForDifferentExhibition()
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

        List<PriceDays> priceDays =
        [
            new()
            {
                ExhibitionDayIds = exhibitionDays.Select(x => x.Id).ToList(), Price = new MultiCurrencyPrice(100, 3)
            }
        ];
        CreatePriceGroupCommand createPriceGroupCommand = new()
        {
            GroupsIds = ["4", "5"], ExhibitionId = exhibitionId.Value, PriceDays = priceDays
        };

        Result<string> pricesId = await SendAsync(createPriceGroupCommand);

        await SendAsync(new GetExhibitionGroupByIdQuery { GroupsId = pricesId.Value });

        Result<int> exhibitionId2 = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organizationId)
        });
        List<ExhibitionDayDto> exhibitionDays2 =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId2.Value });
        List<PriceDays> priceDays2 =
        [
            new()
            {
                ExhibitionDayIds = exhibitionDays2.Select(x => x.Id).ToList(),
                Price = new MultiCurrencyPrice(100, 3)
            }
        ];
        CreatePriceGroupCommand createPriceGroupCommand2 = new()
        {
            GroupsIds = ["4", "5"], ExhibitionId = exhibitionId2.Value, PriceDays = priceDays2
        };
        Result<string> pricesId2 = await SendAsync(createPriceGroupCommand2);

        string createPricesIds = string.Join(",", new List<string> { pricesId.Value, pricesId2.Value });
        DeletePriceGroupCommand deletePricesCommand = new() { PriceIds = createPricesIds };

        // Act
        Func<Task> act = async () => await SendAsync(deletePricesCommand);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        Result<BigPriceDto> data =
            await SendAsync(new GetExhibitionGroupByIdQuery { GroupsId = pricesId.Value });
        data.Should().NotBeNull();
    }
}
