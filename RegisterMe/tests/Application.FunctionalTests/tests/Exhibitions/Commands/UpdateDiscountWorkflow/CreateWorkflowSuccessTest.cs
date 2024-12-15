#region

using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreateWorkflowCommand;
using RegisterMe.Application.Exhibitions.Commands.UpdateDiscountWorkflow;
using RegisterMe.Application.Exhibitions.Queries.GetDiscountById;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.CreateWorkflowCommand;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Domain.Common;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.UpdateDiscountWorkflow;

#region

using static Testing;

#endregion

public class UpdateDiscountWorkflowSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldUpdateDiscountWorkflow()
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


        await WorkflowTestHelper.ReadWorkflowFromFile();
        workflow.Should().NotBeNull();
        UpdateDiscountWorkflowCommand updateDiscountWorkflow = new()
        {
            Workflow = workflow.Single(x => x.WorkflowName == "VelkaPujcenaKlec") ?? throw new Exception(),
            Id = id.Value
        };

        // Act
        Result<int> newId = await SendAsync(updateDiscountWorkflow);

        // Assert
        Func<Task> action = () => SendAsync(new GetDiscountByIdQuery { WorkflowId = id.Value });
        await action.Should().ThrowAsync<NotFoundException>();

        Workflow getWorkflow = await SendAsync(new GetDiscountByIdQuery { WorkflowId = newId.Value });
        getWorkflow.WorkflowName.Should().Be("VelkaPujcenaKlec");
    }
}
