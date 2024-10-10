#region

using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.Organizations.Commands.UpdateOrganization;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.Organizations.Queries.GetOrganizationById;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Organizations.Commands.UpdateOrganization;

#region

using static Testing;

#endregion

public class UpdateOrganizationsSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldUpdateOrganization()
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

        // Act
        await SendAsync(updateOrganizationCommand);

        // Assert
        OrganizationDto updatedOrg = await SendAsync(new GetOrganizationByIdQuery { OrganizationId = organizationId });
        CompareUtils.Equals(updateOrganizationCommand.OrganizationDto, updatedOrg).Should().BeTrue();
    }
}
