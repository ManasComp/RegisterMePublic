#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.UpdateDiscountWorkflow;
using RegisterMe.Application.FunctionalTests.Enums;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.UpdateDiscountWorkflow;

#region

using static Testing;

#endregion

public class UpdateWorkflowValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(-1)]
    [TestCase(0)]
    public async Task ShouldFailUpdateDiscountWorkflow(int id)
    {
        // Arrange

        // Act
        Func<Task> act = async () =>
            await SendAsync(new UpdateDiscountWorkflowCommand { Id = id, Workflow = new Workflow() });

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
