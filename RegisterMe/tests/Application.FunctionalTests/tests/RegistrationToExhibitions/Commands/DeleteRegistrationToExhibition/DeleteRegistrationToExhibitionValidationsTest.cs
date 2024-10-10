#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.DeleteRegistrationToExhibition;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.
    DeleteRegistrationToExhibition;

public class DeleteRegistrationToExhibitionValidationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    public async Task ShouldFailDeleteRegistrationToExhibition(int? registrationToExhibitionId)
    {
        // Arrange
        await RunAsAdministratorAsync();

        // Act
        Func<Task> act = async () => await SendAsync(new DeleteRegistrationToExhibitionCommand
        {
            RegistrationToExhibitionId = registrationToExhibitionId ?? 1
        });

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
