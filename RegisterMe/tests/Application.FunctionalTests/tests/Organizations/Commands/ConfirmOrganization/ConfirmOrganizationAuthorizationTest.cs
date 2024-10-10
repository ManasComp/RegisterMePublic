#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Organizations.Commands.ConfirmOrganization;

#region

using static Testing;

#endregion

public class ConfirmOrganizationAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldConfirmOrganization(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;

        // Act
        ConfirmOrganizationCommand confirmOrganization = new() { OrganizationId = organizationId };
        await RunAsExecutor(runAsSpecificUser);
        Func<Task> act = async () => await SendAsync(confirmOrganization);

        // Assert
        await act.Should().NotThrowAsync();
    }


    [Test]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    public async Task ShouldFailConfirmOrganization(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;

        ConfirmOrganizationCommand confirmOrganization = new() { OrganizationId = organizationId };
        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () => await SendAsync(confirmOrganization);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
