#region

using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.Organizations.Queries.GetOrganizationById;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Organizations.Commands.CreateOrganization;

#region

using static Testing;

#endregion

public class CreateOrganizationsSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldCreateOrganization()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };

        // Act
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;

        // Assert
        organizationId.Should().BeGreaterThan(0);
        GetOrganizationByIdQuery getExhibitionByIdQuery = new() { OrganizationId = organizationId };
        OrganizationDto organizationDto = await SendAsync(getExhibitionByIdQuery);
        organizationDto.Should().NotBeNull();

        bool isSame = CompareUtils.Equals(createOrganizationCommand.CreateOrganizationDto, organizationDto);
        isSame.Should().BeTrue();
    }

    [Test]
    public async Task ShouldFailCreateNextOrganization()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };

        await SendAsync(createOrganizationCommand);

        // Act
        Result<int> newOrgId = await SendAsync(createOrganizationCommand);

        // Assert
        newOrgId.IsSuccess.Should().BeFalse();
    }
}
