#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Organizations.Commands.CreateOrganization;

#region

using static Testing;

#endregion

public class CreateOrganizationAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldCreateOrganization(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };

        // Act
        await RunAsExecutor(runAsSpecificUser);
        Func<Task> act = async () => await SendAsync(createOrganizationCommand);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    public async Task ShouldFailCreateOrganization(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };

        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () => await SendAsync(createOrganizationCommand);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
