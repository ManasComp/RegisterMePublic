#region

using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.UpdateAdvertisement;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetAdvertisementsByExhibitionId;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.UpdateAdvertisement;

#region

using static Testing;

#endregion

public class UpdateAdvertisementSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldUpdateAdvertisement()
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

        CreateAdvertisementCommand createAdvertisementCommand = new()
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibitionId.Value
        };
        Result<int> advertisementId = await SendAsync(createAdvertisementCommand);
        UpsertAdvertisementDto advertisement = AdvertisementDataGenerator.GetAdvertisementDto2();
        UpdateAdvertisementCommand updateAdvertisementCommand = new()
        {
            AdvertisementDto = new UpsertAdvertisementDto
            {
                Description = advertisement.Description,
                Price = advertisement.Price,
                IsDefault = advertisement.IsDefault
            },
            AdvertisementId = advertisementId.Value
        };

        // Act
        await SendAsync(updateAdvertisementCommand);

        // Assert
        GetAdvertisementsByExhibitionIdQuery getAdvertisementsByExhibitionIdQuery =
            new() { ExhibitionId = exhibitionId.Value };
        List<AdvertisementDto> advertisements = await SendAsync(getAdvertisementsByExhibitionIdQuery);
        advertisements.Should().NotBeNull();
        advertisements.Count.Should().Be(1);
        advertisements[0].Should().NotBeNull();
        CompareUtils.Equals(updateAdvertisementCommand.AdvertisementDto, advertisements[0]).Should().BeTrue();
        advertisements[0].Id.Should().Be(advertisementId.Value);
    }
}
