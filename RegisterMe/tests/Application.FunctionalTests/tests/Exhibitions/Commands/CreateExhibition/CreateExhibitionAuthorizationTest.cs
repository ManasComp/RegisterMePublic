#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.CreateExhibition;

#region

using static Testing;

#endregion

public class CreateExhibitionsAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    public async Task ShouldFailCreateExhibition(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;

        await RunAsExecutor(runAsSpecificUser);

        // Act
        CreateExhibitionCommand createExhibitionCommand = new()
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organizationId)
        };
        Func<Task> act = async () => await SendAsync(createExhibitionCommand);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
