#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreateWorkflowCommand;
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

public class DeleteWorkflowSuccessTestAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldDeleteDiscount(RunAsSpecificUser runAsSpecificUser)
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

        // Act
        Result<int> id = await SendAsync(workflowCommandCommand);

        // Assert
        await RunAsExecutor(runAsSpecificUser);
        await SendAsync(
            new Application.Exhibitions.Commands.DeleteDiscountCommand.DeleteDiscountCommand { Id = id.Value });
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    public async Task ShouldNotDeleteDiscount(RunAsSpecificUser runAsSpecificUser)
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

        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () =>
            await SendAsync(
                new Application.Exhibitions.Commands.DeleteDiscountCommand.DeleteDiscountCommand { Id = id.Value });

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
