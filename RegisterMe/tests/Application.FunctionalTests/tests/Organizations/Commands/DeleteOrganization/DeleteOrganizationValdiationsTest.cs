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

public class DeleteOrganizationValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(null)]
    public async Task ShouldDeleteExhibition(int? organizationId)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int defaultOrganizationId = (await SendAsync(createOrganizationCommand)).Value;

        DeleteOrganizationCommand deleteOrganizationCommand = new()
        {
            OrganizationId = organizationId ?? defaultOrganizationId
        };

        // Act
        Func<Task> act = async () => await SendAsync(deleteOrganizationCommand);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }


    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    public async Task ShouldFailDeleteExhibition(int? organizationId)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int defaultOrganizationId = (await SendAsync(createOrganizationCommand)).Value;

        // Act
        DeleteOrganizationCommand confirmOrganization =
            new() { OrganizationId = organizationId ?? defaultOrganizationId };
        Func<Task> act = async () => await SendAsync(confirmOrganization);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
