#region

using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetPrices;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.CreatePrices;

#region

using static Testing;

#endregion

public class CreatePricesSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldCreatePrices()
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
                ExhibitionDayIds = exhibitionDays.Select(x => x.Id).ToList(), Price = new MultiCurrencyPrice(10, 3)
            }
        ];
        CreatePriceGroupCommand createPriceGroupCommand = new()
        {
            GroupsIds = ["4", "5"], ExhibitionId = exhibitionId.Value, PriceDays = priceDays
        };

        // Act
        Result<string> pricesId = await SendAsync(createPriceGroupCommand);

        // Assert
        pricesId.IsSuccess.Should().BeTrue();
        pricesId.Value.Should().NotBeNullOrEmpty();
        pricesId.Value.Split(",").Length.Should().Be(priceDays.Count);
        List<BigPriceDto> prices =
            await SendAsync(new GetPricesQuery { ExhibitionId = exhibitionId.Value });
        prices.Should().NotBeNullOrEmpty();
        prices.Count.Should().Be(priceDays.Count);
        prices.Select(x => x.Prices).First().First().Price.PriceCzk.Should().Be(10);
        prices.Select(x => x.Prices).First().First().Price.PriceEur.Should().Be(3);
        prices.Select(x => x.Prices).First().First().ExhibitionDays.Count.Should().Be(exhibitionDays.Count);
        prices.Select(x => x.Prices).First().First().Groups.Count.Should().Be(2);
        prices.Select(x => x.Prices).First().First().Groups.Should().BeEquivalentTo("4", "5");
    }

    [Test]
    public async Task ShouldNotCreatePricesForDifferentExhibition()
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
        Result<int> exhibitionId2 = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organizationId)
        });
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId2.Value });

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

        // Act
        Result<string> pricesId = await SendAsync(createPriceGroupCommand);

        // Assert
        pricesId.IsSuccess.Should().BeFalse();
    }

    [Test]
    public async Task ShouldNotCreatePricesForRandomExhibitionDayIds()
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

        List<PriceDays> priceDays =
            [new() { ExhibitionDayIds = [1, 5, 7], Price = new MultiCurrencyPrice(100, 3) }];
        CreatePriceGroupCommand createPriceGroupCommand = new()
        {
            GroupsIds = ["4", "5"], ExhibitionId = exhibitionId.Value, PriceDays = priceDays
        };

        // Act
        Result<string> pricesId = await SendAsync(createPriceGroupCommand);

        // Assert
        pricesId.IsSuccess.Should().BeFalse();
    }

    [Test]
    public async Task ShouldNotCreatePricesForExistingPrices()
    {
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
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["4", "5"], ExhibitionId = exhibitionId.Value, PriceDays = priceDays
        });

        CreatePriceGroupCommand createPriceGroupCommand = new()
        {
            GroupsIds = ["4"],
            ExhibitionId = exhibitionId.Value,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds =
                        priceDays.SelectMany(x => x.ExhibitionDayIds).ToList(),
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        };

        // Act
        Result<string> result = await SendAsync(createPriceGroupCommand);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Test]
    public async Task ShouldCreatePricesForExistingPrices()
    {
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
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["4", "5"], ExhibitionId = exhibitionId.Value, PriceDays = priceDays
        });

        CreatePriceGroupCommand createPriceGroupCommand = new()
        {
            GroupsIds = ["6"],
            ExhibitionId = exhibitionId.Value,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds =
                        [exhibitionDays.Select(x => x.Id).First()],
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        };

        // Act
        Result<string> result = await SendAsync(createPriceGroupCommand);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task ShouldNotCreateDuplicatedPrices()
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

        List<PriceDays> priceDays =
            [new() { ExhibitionDayIds = [1, 5, 7], Price = new MultiCurrencyPrice(100, 3) }];
        priceDays.AddRange(priceDays);
        CreatePriceGroupCommand createPriceGroupCommand = new()
        {
            GroupsIds = ["4", "5"], ExhibitionId = exhibitionId.Value, PriceDays = priceDays
        };

        // Act
        Result<string> pricesId = await SendAsync(createPriceGroupCommand);

        // Assert
        pricesId.IsSuccess.Should().BeFalse();
    }
}
