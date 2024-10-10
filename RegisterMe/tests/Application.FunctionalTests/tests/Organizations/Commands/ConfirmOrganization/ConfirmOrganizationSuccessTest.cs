#region

using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.Organizations.Queries.GetOrganizationById;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Organizations.Commands.ConfirmOrganization;

#region

using static Testing;

#endregion

public class ConfirmOrganizationsSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldConfirmOrganization()
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
        await RunAsAdministratorAsync();
        await SendAsync(confirmOrganization);
        OrganizationDto updatedOrg = await SendAsync(new GetOrganizationByIdQuery { OrganizationId = organizationId });

        // Assert
        updatedOrg.IsConfirmed.Should().BeTrue();
    }

    [Test]
    public async Task ShouldFailDoubleConfirmOrganization()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;
        await RunAsAdministratorAsync();

        // Act
        ConfirmOrganizationCommand confirmOrganization = new() { OrganizationId = organizationId };
        Result result1 = await SendAsync(confirmOrganization);
        result1.IsSuccess.Should().BeTrue();

        Result result2 = await SendAsync(confirmOrganization);
        result2.IsSuccess.Should().BeFalse();
        OrganizationDto updatedOrg = await SendAsync(new GetOrganizationByIdQuery { OrganizationId = organizationId });

        // Assert
        updatedOrg.IsConfirmed.Should().BeTrue();
    }
}
