#region

using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.Organizations.Commands.UpdateOrganization;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.Organizations.Queries.GetOrganizationById;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Organizations;

#region

using static Testing;

#endregion

public class CreateRegistrationToExhibition(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldUpdateOrganizationTest()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationID = (await SendAsync(createOrganizationCommand)).Value;
        CreateOrganizationDto updatedOranization = OrganizationDataGenerator.GetOrganizationDto2(user);

        UpdateOrganizationCommand updateOrganizationCommand = new()
        {
            OrganizationDto = new UpdateOrganizationDto
            {
                Id = organizationID,
                Name = updatedOranization.Name,
                Email = updatedOranization.Email,
                TelNumber = updatedOranization.TelNumber,
                Website = updatedOranization.Website,
                Address = updatedOranization.Address
            }
        };

        // Act
        await SendAsync(updateOrganizationCommand);


        // Assert
        organizationID.Should().BeGreaterThan(0);
        GetOrganizationByIdQuery getExhibitionByIdQuery = new() { OrganizationId = organizationID };
        OrganizationDto organizationDto = await SendAsync(getExhibitionByIdQuery);
        organizationDto.Should().NotBeNull();

        bool isSame = CompareUtils.Equals(updatedOranization, updateOrganizationCommand.OrganizationDto);
        isSame.Should().BeTrue();
    }
}
