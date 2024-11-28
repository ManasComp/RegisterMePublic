#region

using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreateWorkflowCommand;
using RegisterMe.Application.Exhibitions.Queries.GetDiscountById;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.CreateWorkflowCommand;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Domain.Common;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.DeleteDiscountCommand;

#region

using static Testing;

#endregion

public class DeleteWorkflowSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldDeleteDiscount()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;
        Result<int> exhibitionId = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organizationId)
        });

        List<Workflow>? workflow = await WorkflowTestHelper.ReadWorkflowFromFile();
        workflow.Should().NotBeNull();
        CreateWorkflowCommandCommand workflowCommandCommand = new()
        {
            Workflow = workflow?.Single(x => x.WorkflowName == "MnozstevniSleva") ?? throw new Exception(),
            ExhibitionId = exhibitionId.Value
        };
        Result<int> id = await SendAsync(workflowCommandCommand);

        await SendAsync(new GetDiscountByIdQuery { WorkflowId = id.Value });

        // Act
        Application.Exhibitions.Commands.DeleteDiscountCommand.DeleteDiscountCommand command = new() { Id = id.Value };

        // Assert
        await SendAsync(command);
        Func<Task> action = () => SendAsync(new GetDiscountByIdQuery { WorkflowId = id.Value });
        await action.Should().ThrowAsync<NotFoundException>();
    }
}
