#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.UpdateAdvertisement;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.UpdateAdvertisement;

#region

using static Testing;

#endregion

public class UpdateAdvertisementAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    public async Task ShouldFailUpdateExhibition(RunAsSpecificUser runAsSpecificUser)
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
        await RunAsExecutor(runAsSpecificUser);

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
        Func<Task> act = async () => await SendAsync(updateAdvertisementCommand);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    public async Task ShouldUpdateAdvertisement(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;

        // Act
        Result<int> exhibitionId = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organizationId)
        });

        Result<int> advertisementId = await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibitionId.Value
        });
        await RunAsExecutor(runAsSpecificUser);

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
        Func<Task> act = async () => await SendAsync(updateAdvertisementCommand);

        // Assert
        await act.Should().NotThrowAsync<ForbiddenAccessException>();
    }
}
