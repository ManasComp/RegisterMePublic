#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.Organizations.Commands.UpdateOrganization;
using RegisterMe.Application.Organizations.Dtos;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Organizations.Commands.UpdateOrganization;

#region

using static Testing;

#endregion

public class UpdateOrganizationAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldUpdateOrganization(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;

        // Act
        CreateOrganizationDto nextOrganization = OrganizationDataGenerator.GetOrganizationDto2(user);
        UpdateOrganizationCommand updateOrganizationCommand = new()
        {
            OrganizationDto = new UpdateOrganizationDto
            {
                Address = nextOrganization.Address,
                Email = nextOrganization.Email,
                TelNumber = nextOrganization.TelNumber,
                Website = nextOrganization.Website,
                Name = nextOrganization.Name,
                Id = organizationId
            }
        };
        await RunAsExecutor(runAsSpecificUser);
        Func<Task> act = async () => await SendAsync(updateOrganizationCommand);

        // Assert
        await act.Should().NotThrowAsync();
    }


    [Test]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    public async Task ShouldFailUpdateOrganization(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;

        CreateOrganizationDto nextOrganization = OrganizationDataGenerator.GetOrganizationDto2(user);
        UpdateOrganizationCommand updateOrganizationCommand = new()
        {
            OrganizationDto = new UpdateOrganizationDto
            {
                Address = nextOrganization.Address,
                Email = nextOrganization.Email,
                TelNumber = nextOrganization.TelNumber,
                Website = nextOrganization.Website,
                Name = nextOrganization.Name,
                Id = organizationId
            }
        };
        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () => await SendAsync(updateOrganizationCommand);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
