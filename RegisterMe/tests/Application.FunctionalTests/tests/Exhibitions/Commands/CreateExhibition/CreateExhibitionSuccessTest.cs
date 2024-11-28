#region

using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionById;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.CreateExhibition;

#region

using static Testing;

#endregion

public class CreateExhibitionsSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldCreateExhibition()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;
        CreateExhibitionCommand createExhibitionCommand = new()
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organizationId)
        };

        // Act
        int exhibitionId = (await SendAsync(createExhibitionCommand)).Value;

        // Assert
        exhibitionId.Should().BeGreaterThan(0);
        GetExhibitionByIdQuery getExhibitionByIdQuery = new() { ExhibitionId = exhibitionId };
        BriefExhibitionDto briefExhibition = await SendAsync(getExhibitionByIdQuery);
        briefExhibition.Should().NotBeNull();

        bool isSame = CompareUtils.Equals(briefExhibition, createExhibitionCommand.CreateExhibitionDto);
        isSame.Should().BeTrue();
    }
}
