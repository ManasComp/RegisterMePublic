#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.Organizations.Commands.DeleteOrganization;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Organizations.Commands.DeleteOrganization;

#region

using static Testing;

#endregion

public class DeleteOrganizationAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldDeleteOrganization(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;

        await RunAsExecutor(runAsSpecificUser);
        DeleteOrganizationCommand deleteOrganizationCommand = new() { OrganizationId = organizationId };

        // Act
        Func<Task> act = async () => await SendAsync(deleteOrganizationCommand);

        // Assert
        await act.Should().NotThrowAsync();
    }


    [Test]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    public async Task ShouldFailDeleteOrganization(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;

        // Act
        await RunAsExecutor(runAsSpecificUser);
        DeleteOrganizationCommand deleteOrganizationCommand = new() { OrganizationId = organizationId };
        Func<Task> act = async () => await SendAsync(deleteOrganizationCommand);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
