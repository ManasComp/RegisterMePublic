#region

using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.DeleteAdvertisementById;
using RegisterMe.Application.Exhibitions.Queries.GetAdvertisementById;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.DeleteAdvertisementById;

#region

using static Testing;

#endregion

public class DeleteAdvertisementSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldFailDeleteAdvertisement()
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

        // Act
        await SendAsync(new DeleteAdvertisementCommand { AdvertisementId = advertisementId.Value });

        // Assert
        GetAdvertisementByIdQuery query = new() { AdvertisementId = advertisementId.Value };
        Func<Task> act = async () => await SendAsync(query);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
