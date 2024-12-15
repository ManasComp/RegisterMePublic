#region

using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Commands.UpdatePriceGroup;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionGroupById;
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

public class UpdatePricesSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    public enum Groups
    {
        ZeroGroup,
        OneGroup,
        TwoGroups,
        ThreeGroups,
        FourGroups
    }

    private static List<string> GetGroups(Groups priceDaysTestEnum)
    {
        return priceDaysTestEnum switch
        {
            Groups.ZeroGroup => [],
            Groups.OneGroup => ["1"],
            Groups.TwoGroups => ["1", "2"],
            Groups.ThreeGroups => ["1", "2", "3"],
            Groups.FourGroups => ["1", "2", "3", "4"],
            _ => throw new ArgumentOutOfRangeException(nameof(priceDaysTestEnum), priceDaysTestEnum, null)
        };
    }

    [Test]
    [TestCase(Groups.OneGroup)]
    [TestCase(Groups.TwoGroups)]
    [TestCase(Groups.ThreeGroups)]
    [TestCase(Groups.FourGroups)]
    public async Task ShouldUpdatePrices(Groups group)
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
        List<string> groups = GetGroups(group);
        UpdatePriceGroupCommand updatePricesCommand = new()
        {
            OriginalPricesId = pricesId.Value,
            GroupsIds = groups,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds =
                        [exhibitionDays.Select(x => x.Id).First()],
                    Price = new MultiCurrencyPrice(15, 4)
                }
            ]
        };

        // Act
        Result<string> updatedPrices = await SendAsync(updatePricesCommand);

        // Assert
        Result<BigPriceDto> getPrices =
            await SendAsync(new GetExhibitionGroupByIdQuery { GroupsId = updatedPrices.Value });

        getPrices.Value.Should().NotBeNull();
        getPrices.Value.Groups.Should().BeEquivalentTo(string.Join(",", groups));
        getPrices.Value.Prices.Should().NotBeNull();
        getPrices.Value.Prices.Should().NotBeEmpty();
        getPrices.Value.Prices.First().Price.PriceCzk.Should().Be(15);
        getPrices.Value.Prices.First().Price.PriceEur.Should().Be(4);
        getPrices.Value.Prices.First().ExhibitionDays.Should().NotBeEmpty();
        getPrices.Value.Prices.First().ExhibitionDays.Count.Should().Be(1);
    }
}
