#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Users.Command;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Users.Commands.DeletePersonalData;

#region

using static Testing;

#endregion

public class DeletePersonalDataAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldDeletePersonalData(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string id = await RunAsOndrejAsync();
        CreateExhibitorCommand createExhibitorCommand = new()
        {
            UserId = id, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
        };
        await RunAsExecutor(runAsSpecificUser);
        await SendAsync(createExhibitorCommand);

        // Act
        Func<Task> act = async () => await SendAsync(new DeletePersonalDataCommand { UserId = id });

        // Assert
        await act.Should().NotThrowAsync();
    }


    [Test]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    public async Task ShouldFailUpdateExhibitor(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string id = await RunAsOndrejAsync();
        CreateExhibitorCommand createExhibitorCommand = new()
        {
            UserId = id, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
        };
        await SendAsync(createExhibitorCommand);
        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () => await SendAsync(new DeletePersonalDataCommand { UserId = id });

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
