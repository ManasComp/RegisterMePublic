#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.DeleteDiscountCommand;

#region

using static Testing;

#endregion

public class DeleteWorkflowValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(-1)]
    [TestCase(0)]
    public async Task ShouldFailDeleteDiscount(int id)
    {
        // Arrange

        // Act
        Func<Task> act = async () =>
            await SendAsync(
                new Application.Exhibitions.Commands.DeleteDiscountCommand.DeleteDiscountCommand { Id = id });

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
