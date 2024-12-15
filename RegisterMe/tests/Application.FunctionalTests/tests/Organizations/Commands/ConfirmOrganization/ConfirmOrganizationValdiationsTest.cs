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

public class ConfirmOrganizationValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(null)]
    public async Task ShouldConfirmExhibition(int? organizationId)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int defaultOrganizationId = (await SendAsync(createOrganizationCommand)).Value;

        // Act
        ConfirmOrganizationCommand confirmOrganization =
            new() { OrganizationId = organizationId ?? defaultOrganizationId };
        await RunAsAdministratorAsync();
        Func<Task> act = async () => await SendAsync(confirmOrganization);

        // Assert
        await act.Should().NotThrowAsync();
    }


    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    public async Task ShouldFailConfirmExhibition(int? organizationId)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int defaultOrganizationId = (await SendAsync(createOrganizationCommand)).Value;
        ConfirmOrganizationCommand confirmOrganization =
            new() { OrganizationId = organizationId ?? defaultOrganizationId };
        await RunAsAdministratorAsync();

        // Act
        Func<Task> act = async () => await SendAsync(confirmOrganization);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
