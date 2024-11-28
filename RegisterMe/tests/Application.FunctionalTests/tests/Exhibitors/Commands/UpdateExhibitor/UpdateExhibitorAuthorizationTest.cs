#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.Exhibitors.Commands.UpdateExhibitor;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitors.Commands.UpdateExhibitor;

#region

using static Testing;

#endregion

public class UpdateExhibitorExhibitionsAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldUpdateExhibitor(RunAsSpecificUser runAsSpecificUser)
    {
        string id = await RunAsOndrejAsync();

        // Arrange
        CreateExhibitorCommand createExhibitorCommand = new()
        {
            UserId = id, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
        };
        await RunAsExecutor(runAsSpecificUser);
        await SendAsync(createExhibitorCommand);
        UpdateExhibitorCommand updateExhibitorCommand = new()
        {
            Exhibitor = ExhibitorDataGenerator.GetExhibitorDto2(), AspNetUserId = id
        };

        // Act
        Func<Task> act = async () => await SendAsync(updateExhibitorCommand);

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
        UpdateExhibitorCommand updateExhibitorCommand = new()
        {
            Exhibitor = ExhibitorDataGenerator.GetExhibitorDto2(), AspNetUserId = id
        };

        // Act
        Func<Task> act = async () => await SendAsync(updateExhibitorCommand);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
