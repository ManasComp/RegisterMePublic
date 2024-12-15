#region

using RegisterMe.Application.CatRegistrations.Commands.DeleteCatRegistration;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.Enums;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.CatRegistrations.Commands.
    DeleteCatRegistration;

public class ShouldDeleteCatRegistrationValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(-1)]
    [TestCase(0)]
    public async Task ShouldFailCreateCatRegistration(int catRegistrationToDelete)
    {
        // Arrange

        // Act
        Func<Task> getCatRegistration = async () =>
            await SendAsync(new DeleteCatRegistrationCommand { CatRegistrationId = catRegistrationToDelete });
        // Assert
        await getCatRegistration.Should().ThrowAsync<ValidationException>();
    }
}
