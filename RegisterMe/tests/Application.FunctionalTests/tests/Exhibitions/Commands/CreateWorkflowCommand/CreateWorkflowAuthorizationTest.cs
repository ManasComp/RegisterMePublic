#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreateWorkflowCommand;
using RegisterMe.Application.Exhibitions.Queries.GetDiscountById;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Domain.Common;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.CreateWorkflowCommand;

#region

using static Testing;

#endregion

public class CreateWorkflowAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldsCreateWorkflow(RunAsSpecificUser runAsSpecificUser)
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

        await RunAsExecutor(runAsSpecificUser);

        CreateWorkflowCommandCommand workflowCommandCommand = new()
        {
            Workflow = workflow?.Single(x => x.WorkflowName == "MnozstevniSleva") ?? throw new Exception(),
            ExhibitionId = exhibitionId.Value
        };

        // Act
        Result<int> id = await SendAsync(workflowCommandCommand);

        // Assert
        Workflow getWOrkflow = await SendAsync(new GetDiscountByIdQuery { WorkflowId = id.Value });
        getWOrkflow.WorkflowName.Should().Be("MnozstevniSleva");
        getWOrkflow.Rules.Should().NotBeNull();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    public async Task ShouldFailCreateWorkflow(RunAsSpecificUser runAsSpecificUser)
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

        await RunAsExecutor(runAsSpecificUser);

        CreateWorkflowCommandCommand workflowCommandCommand = new()
        {
            Workflow = workflow?.Single(x => x.WorkflowName == "MnozstevniSleva") ?? throw new Exception(),
            ExhibitionId = exhibitionId.Value
        };

        // Act
        Func<Task> act = async () =>
            await SendAsync(workflowCommandCommand);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
