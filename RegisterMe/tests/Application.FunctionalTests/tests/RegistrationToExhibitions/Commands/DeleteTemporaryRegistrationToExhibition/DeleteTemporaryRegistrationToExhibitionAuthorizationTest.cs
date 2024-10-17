#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.DeleteTemporaryRegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.
    DeleteTemporaryRegistrationToExhibition;

#region

using static Testing;

#endregion

public class DeleteTemporaryRegistrationToExhibitionCommandAuthorizationTest(DatabaseTypes databaseType)
    : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    public async Task ShouldDeleteTemporaryRegistrationToExhibition(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        (List<ExhibitionDayDto> _, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
        await RunAsVojtaAsync();

        RegistrationToExhibitionDto registrationToExhibition = await SendAsync(
            new GetRegistrationToExhibitionByIdQuery { RegistrationToExhibitionId = registrationToExhibitionId.Value });

        await RunAsExecutor(runAsSpecificUser);

        // Act
        Result result = await SendAsync(new DeleteTemporaryRegistrationToExhibitionCommand
        {
            ExhibitionId = registrationToExhibition.ExhibitionId, WebAddress = "https://www.google.com"
        });

        // Assert
        result.IsSuccess.Should().BeTrue();
    }


    [Test]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    [TestCase(RunAsSpecificUser.RunAsVojta)]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    public async Task ShouldFailDeleteTemporaryRegistrationToExhibition(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        (List<ExhibitionDayDto> _, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();

        await RunAsVojtaAsync();
        RegistrationToExhibitionDto registrationToExhibition = await SendAsync(
            new GetRegistrationToExhibitionByIdQuery { RegistrationToExhibitionId = registrationToExhibitionId.Value });

        await RunAsExecutor(runAsSpecificUser);


        // Act
        Func<Task> balance = async () =>
            await SendAsync(new DeleteTemporaryRegistrationToExhibitionCommand
            {
                ExhibitionId = registrationToExhibition.ExhibitionId, WebAddress = "https://www.google.com"
            });

        // Assert
        await balance.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
