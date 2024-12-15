#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
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

public class CreatePricesValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    public enum PriceDaysTestEnum
    {
        EmptyExhibitionDayIds,
        Empty,
        TwoGood
    }

    private static List<PriceDays> GetPriceDays(PriceDaysTestEnum priceDaysTestEnum)
    {
        return priceDaysTestEnum switch
        {
            PriceDaysTestEnum.EmptyExhibitionDayIds =>
                [new PriceDays { ExhibitionDayIds = [], Price = new MultiCurrencyPrice(100, 3) }],
            PriceDaysTestEnum.Empty => [],
            PriceDaysTestEnum.TwoGood =>
            [
                new PriceDays { ExhibitionDayIds = [], Price = new MultiCurrencyPrice(5, 4) },
                new PriceDays { ExhibitionDayIds = [], Price = new MultiCurrencyPrice(4, 3) }
            ],
            _ => throw new ArgumentOutOfRangeException(nameof(priceDaysTestEnum), priceDaysTestEnum, null)
        };
    }

    [Test]
    [TestCase(0, null, null)]
    [TestCase(null, new string[] { }, null)]
    [TestCase(null, null, PriceDaysTestEnum.EmptyExhibitionDayIds)]
    [TestCase(null, null, PriceDaysTestEnum.Empty)]
    public async Task ShouldFailValidations(int? exhibitionIdParam,
        string[]? groupsParam, PriceDaysTestEnum? priceDaysTestEnum)
    {
        // Arrange
        List<PriceDays>? priceDaysParam = null;
        if (priceDaysTestEnum != null)
        {
            priceDaysParam = GetPriceDays(priceDaysTestEnum.Value);
        }

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

        exhibitionIdParam ??= exhibitionId.Value;
        List<ExhibitionDayDto> exhibitionDays = await SendAsync(
            new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId.Value }
        );
        priceDaysParam ??=
        [
            new PriceDays
            {
                ExhibitionDayIds = exhibitionDays.Select(x => x.Id).ToList(), Price = new MultiCurrencyPrice(100, 3)
            }
        ];
        groupsParam ??= ["1"];

        CreatePriceGroupCommand createPriceGroupCommand = new()
        {
            GroupsIds = groupsParam.ToList(),
            ExhibitionId = exhibitionIdParam.Value,
            PriceDays = priceDaysParam.ToList()
        };

        // Act
        Func<Task> act = async () => await SendAsync(createPriceGroupCommand);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    [TestCase(null, null, null)]
    [TestCase(null, new[] { "2", "3" }, null)]
    public async Task ShouldNotFailValidations(int? exhibitionIdParam,
        string[]? groupsParam, List<PriceDays>? priceDaysParam)
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

        exhibitionIdParam ??= exhibitionId.Value;
        List<ExhibitionDayDto> exhibitionDays = await SendAsync(
            new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId.Value }
        );
        priceDaysParam ??=
        [
            new PriceDays
            {
                ExhibitionDayIds = exhibitionDays.Select(x => x.Id).ToList(), Price = new MultiCurrencyPrice(10, 3)
            }
        ];
        groupsParam ??= ["1"];

        // Act
        CreatePriceGroupCommand createPriceGroupCommand = new()
        {
            GroupsIds = groupsParam.ToList(), ExhibitionId = exhibitionIdParam.Value, PriceDays = priceDaysParam
        };
        Func<Task> act = async () => await SendAsync(createPriceGroupCommand);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Test]
    public async Task ShouldNotCreatePricesForDuplicatedIds()
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

        // Act
        List<int> ids = exhibitionDays.Select(x => x.Id).ToList();
        ids.AddRange(ids);
        List<PriceDays> priceDays = [new() { ExhibitionDayIds = ids, Price = new MultiCurrencyPrice(100, 3) }];

        CreatePriceGroupCommand createPriceGroupCommand = new()
        {
            GroupsIds = ["4", "5"], ExhibitionId = exhibitionId.Value, PriceDays = priceDays
        };
        Func<Task> act = async () => await SendAsync(createPriceGroupCommand);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
