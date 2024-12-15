#region

using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Commands.PublishExhibition;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Enums;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitions;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Queries.GetExhibitions;

#region

using static Testing;

#endregion

public class GetExhibitionsSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(1, 10, 1, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    [TestCase(2, 50, null, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    [TestCase(2, 50, null, null, OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    [TestCase(1, 10, 1, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.Future)]
    [TestCase(2, 50, null, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.CanRegisterTo)]
    public async Task ShouldGetExhibitionsNoExists(int pageNumber, int pageSize, int? organizationId,
        string? searchString, OrganizationPublishStatus organizationPublishStatus,
        ExhibitionRegistrationStatus exhibitionStatus)
    {
        // Arrange
        string user = await RunAsOndrejAsync();

        // Act
        GetExhibitionsQuery query = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrganizationId = organizationId,
            UserId = user,
            SearchString = searchString,
            OrganizationPublishStatus = organizationPublishStatus,
            ExhibitionStatus = exhibitionStatus
        };

        IReadOnlyCollection<ExhibitionDto> items = (await SendAsync(query)).Items;

        // Assert
        items.Should().BeEmpty();
    }

    [Test]
    [TestCase(1, 2, null, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    [TestCase(1, 50, null, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    public async Task ShouldReturnCorrectPublishedExhibitions(int pageNumber, int pageSize, int? organizationId,
        string? searchString, OrganizationPublishStatus organizationPublishStatus,
        ExhibitionRegistrationStatus exhibitionStatus)
    {
        // Arrange
        string user1 = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user1)
        })).Value;


        string user2 = await RunAsSabrinaAsync();
        int organization2 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto2(user2)
        })).Value;

        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization2 });

        await RunAsOndrejAsync();
        Result<int> exhibition1 = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        });
        exhibition1.IsSuccess.Should().BeTrue();
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibition1.Value });
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["1", "2"],
            ExhibitionId = exhibition1.Value,
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
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1.Value
        });
        (await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1.Value })).IsSuccess.Should()
            .BeTrue();

        await RunAsSabrinaAsync();
        Result<int> exhibition2 = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization2)
        });
        exhibition2.IsSuccess.Should().BeTrue();
        List<ExhibitionDayDto> exhibitionDays2 =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibition2.Value });
        Result<string> prices = await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["1", "2"],
            ExhibitionId = exhibition2.Value,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = exhibitionDays2.Select(x => x.Id).ToList(),
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        prices.IsSuccess.Should().BeTrue();
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition2.Value
        });
        (await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition2.Value })).IsSuccess.Should()
            .BeTrue();

        // Act
        GetExhibitionsQuery query = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrganizationId = organizationId,
            UserId = null,
            SearchString = searchString,
            OrganizationPublishStatus = organizationPublishStatus,
            ExhibitionStatus = exhibitionStatus
        };

        IReadOnlyCollection<ExhibitionDto> items = (await SendAsync(query)).Items;

        // Assert
        items.Should().NotBeEmpty();
        items.Count.Should().BeLessOrEqualTo(pageSize);
        items.Count.Should().Be(2);
        items.Select(x => x.Id).Should().Contain(exhibition1.Value);
        items.Select(x => x.Id).Should().Contain(exhibition2.Value);
    }


    [Test]
    [TestCase(1, 2, null, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    [TestCase(1, 50, null, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    public async Task ShouldReturnCorrectPublishedExhibitionsForUser(int pageNumber, int pageSize, int? organizationId,
        string? searchString, OrganizationPublishStatus organizationPublishStatus,
        ExhibitionRegistrationStatus exhibitionStatus)
    {
        // Arrange
        string user1 = await createOrganizationWIthExhibition1();
        await CreateOrganizationWIthExhibition2();

        // Act
        await RunAsOndrejAsync();
        GetExhibitionsQuery query = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrganizationId = organizationId,
            UserId = user1,
            SearchString = searchString,
            OrganizationPublishStatus = organizationPublishStatus,
            ExhibitionStatus = exhibitionStatus
        };

        IReadOnlyCollection<ExhibitionDto> items = (await SendAsync(query)).Items;

        // Assert
        items.Should().BeEmpty();
        items.Count.Should().BeLessOrEqualTo(pageSize);
        items.Count.Should().Be(0);
    }

    private static async Task CreateOrganizationWIthExhibition2()
    {
        string user2 = await RunAsSabrinaAsync();
        int organization2 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto2(user2)
        })).Value;
        Result<int> exhibition2 = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization2)
        });
        List<ExhibitionDayDto> exhibitionDays2 =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibition2.Value });
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["1", "2"],
            ExhibitionId = exhibition2.Value,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = exhibitionDays2.Select(x => x.Id).ToList(),
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization2 });
        await RunAsSabrinaAsync();
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition2.Value
        });
        (await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition2.Value })).IsSuccess.Should()
            .BeTrue();
    }

    private static async Task<string> createOrganizationWIthExhibition1()
    {
        string user1 = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user1)
        })).Value;
        Result<int> exhibition1 = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        });
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibition1.Value });
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["1", "2"],
            ExhibitionId = exhibition1.Value,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = exhibitionDays.Select(x => x.Id).ToList(),
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });
        await RunAsOndrejAsync();
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1.Value
        });
        (await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1.Value })).IsSuccess.Should()
            .BeTrue();
        return user1;
    }

    [Test]
    [TestCase(1, 1, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    [TestCase(1, 50, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    public async Task ShouldReturnCorrectPublishedExhibitionsForOrganization(int pageNumber, int pageSize,
        string? searchString, OrganizationPublishStatus organizationPublishStatus,
        ExhibitionRegistrationStatus exhibitionStatus)
    {
        // Arrange
        string user1 = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user1)
        })).Value;
        Result<int> exhibition1 = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        });
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibition1.Value });
        Result<string> prices = await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["1", "2"],
            ExhibitionId = exhibition1.Value,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = exhibitionDays.Select(x => x.Id).ToList(),
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        prices.IsSuccess.Should().BeTrue();
        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });
        await RunAsOndrejAsync();
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1.Value
        });
        (await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1.Value })).IsSuccess.Should()
            .BeTrue();

        await CreateOrganizationWIthExhibition2();


        // Act
        GetExhibitionsQuery query = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrganizationId = organization1,
            UserId = null,
            SearchString = searchString,
            OrganizationPublishStatus = organizationPublishStatus,
            ExhibitionStatus = exhibitionStatus
        };

        IReadOnlyCollection<ExhibitionDto> items = (await SendAsync(query)).Items;

        // Assert
        items.Should().NotBeEmpty();
        items.Count.Should().BeLessOrEqualTo(pageSize);
        items.Count.Should().Be(1);
        items.Select(x => x.Id).Should().Contain(exhibition1.Value);
    }
}
