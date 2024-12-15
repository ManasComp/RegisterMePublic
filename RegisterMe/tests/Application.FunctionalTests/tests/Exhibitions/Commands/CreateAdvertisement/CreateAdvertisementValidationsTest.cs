#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.CreateAdvertisement;

#region

using static Testing;

#endregion

public class CreateAdvertisementValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase("", 10, 5, true)]
    [TestCase("Description", -1, 5, false)]
    [TestCase("", 5, 10, true)]
    public async Task ShouldFailCreateAdvertisements(string description, int priceCzk, int priceEur, bool idDefault)
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

        // Act
        CreateAdvertisementCommand createAdvertisementCommand = new()
        {
            Advertisement = new UpsertAdvertisementDto
            {
                Description = description,
                IsDefault = idDefault,
                Price = new MultiCurrencyPrice(priceCzk, priceEur)
            },
            ExhibitionId = exhibitionId.Value
        };
        Func<Task> act = async () => await SendAsync(createAdvertisementCommand);
        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    [TestCase("Description", 40, 10, true)]
    [TestCase("Description", 8, 6, false)]
    public async Task ShouldCreateAdvertisement(string description, int priceCzk, int priceEur, bool idDefault)
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

        // Act
        CreateAdvertisementCommand createAdvertisementCommand = new()
        {
            Advertisement = new UpsertAdvertisementDto
            {
                Description = description,
                IsDefault = idDefault,
                Price = new MultiCurrencyPrice(priceCzk, priceEur)
            },
            ExhibitionId = exhibitionId.Value
        };
        Func<Task> act = async () => await SendAsync(createAdvertisementCommand);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
