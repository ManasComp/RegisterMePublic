#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.UpdateAdvertisement;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.UpdateAdvertisement;

#region

using static Testing;

#endregion

public class UpdateAdvertisementValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase("", 10, 1, true)]
    [TestCase("Description", -1, 1, false)]
    public async Task ShouldFailValidations(string description, int priceCzk, int priceEur, bool idDefautl)
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

        Result<int> advertisementId = await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibitionId.Value
        });

        UpdateAdvertisementCommand updateAdvertisementCommand = new()
        {
            AdvertisementDto = new UpsertAdvertisementDto
            {
                Description = description,
                IsDefault = idDefautl,
                Price = new MultiCurrencyPrice(priceCzk, priceEur)
            },
            AdvertisementId = advertisementId.Value
        };

        // Act
        Func<Task> act = async () => await SendAsync(updateAdvertisementCommand);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    [TestCase("Description", 11, 10, true)]
    [TestCase("Description", 45, 10, false)]
    public async Task ShouldNotFailValidations(string description, int priceCzk, int priceEur, bool idDefautl)
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

        Result<int> advertisementId = await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibitionId.Value
        });

        UpdateAdvertisementCommand updateAdvertisementCommand = new()
        {
            AdvertisementDto = new UpsertAdvertisementDto
            {
                Description = description,
                IsDefault = idDefautl,
                Price = new MultiCurrencyPrice(priceCzk, priceEur)
            },
            AdvertisementId = advertisementId.Value
        };

        // Act
        Func<Task> act = async () => await SendAsync(updateAdvertisementCommand);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
