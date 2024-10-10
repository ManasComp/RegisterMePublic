#region

using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.Organizations.Commands.DeleteOrganization;
using RegisterMe.Application.Organizations.Queries.GetOrganizationById;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Organizations.Commands.DeleteOrganization;

#region

using static Testing;

#endregion

public class DeleteOrganizationsSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldDeleteOrganization()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;

        DeleteOrganizationCommand deleteOrganization = new() { OrganizationId = organizationId };

        // Act
        Result result = await SendAsync(deleteOrganization);

        // Assert
        result.IsSuccess.Should().BeTrue();
        Func<Task> act = async () => await SendAsync(new GetOrganizationByIdQuery { OrganizationId = organizationId });
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldFailDeleteConfirmedOrganization()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;

        // Act
        DeleteOrganizationCommand deleteOrganization = new() { OrganizationId = organizationId };
        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organizationId });
        await RunAsOndrejAsync();
        Result result = await SendAsync(deleteOrganization);

        // Assert
        result.IsSuccess.Should().BeFalse();
        Func<Task> act = async () => await SendAsync(new GetOrganizationByIdQuery { OrganizationId = organizationId });
        await act.Should().NotThrowAsync();
    }
}
