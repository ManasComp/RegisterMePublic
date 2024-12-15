#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.UpdatePaymentWorkflow;
using RegisterMe.Application.FunctionalTests.Enums;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.UpdatePaymentWorkflow;

#region

using static Testing;

#endregion

public class UpdatePaymentValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(-1)]
    [TestCase(0)]
    public async Task ShouldFailUpdatePaymentWorkflow(int id)
    {
        // Arrange

        // Act
        Func<Task> act = async () =>
            await SendAsync(new UpdatePaymentWorkflowCommand { ExhibitionId = id, PaymentWorkflow = new Workflow() });

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
