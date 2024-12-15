#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitors.Commands.CreateExhibitor;

#region

using static Testing;

#endregion

public class CreateExhibitorAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldCreateExhibitor(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string id = await RunAsOndrejAsync();
        await RunAsExecutor(runAsSpecificUser);

        CreateExhibitorCommand createExhibitorCommand = new()
        {
            UserId = id, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
        };

        // Act
        Func<Task> act = async () => await SendAsync(createExhibitorCommand);

        // Assert
        await act.Should().NotThrowAsync();
    }


    [Test]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    public async Task ShouldFailCreateExhibitor(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string id = await RunAsOndrejAsync();
        await RunAsExecutor(runAsSpecificUser);

        CreateExhibitorCommand createExhibitorCommand = new()
        {
            UserId = id, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
        };

        // Act
        Func<Task> act = async () =>
            await SendAsync(createExhibitorCommand);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
