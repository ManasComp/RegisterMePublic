#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.DeleteTemporaryRegistrationToExhibition;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.
    DeleteTemporaryRegistrationToExhibition;

public class DeleteTemporaryRegistrationToExhibitionCommandValidationTest(DatabaseTypes databaseType)
    : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    public async Task ShouldDeleteTemporaryRegistrationToExhibition(int exhibitionId)
    {
        // Arrange
        await RunAsAdministratorAsync();

        // Act
        Func<Task> act = async () => await SendAsync(
            new DeleteTemporaryRegistrationToExhibitionCommand
            {
                ExhibitionId = exhibitionId, WebAddress = "https://www.google.com"
            });

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
