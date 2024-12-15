#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.UpdatePaymentWorkflow;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.CreateWorkflowCommand;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Domain.Common;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.UpdatePaymentWorkflow;

#region

using static Testing;

#endregion

public class UpdatePaymentWorkflowSuccessTestAuthorizationTest(DatabaseTypes databaseType)
    : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldUpdatePaymentWorkflow(RunAsSpecificUser runAsSpecificUser)
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
        UpdatePaymentWorkflowCommand workflowCommandCommand = new()
        {
            PaymentWorkflow = workflow!.First(), ExhibitionId = exhibitionId.Value
        };

        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () => await SendAsync(workflowCommandCommand);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    public async Task ShouldFailUpdatePaymentWorkflow(RunAsSpecificUser runAsSpecificUser)
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

        UpdatePaymentWorkflowCommand workflowCommandCommand = new()
        {
            PaymentWorkflow = workflow!.First(), ExhibitionId = exhibitionId.Value
        };

        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () =>
            await SendAsync(workflowCommandCommand);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
