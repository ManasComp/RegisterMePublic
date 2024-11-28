#region

using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.UpdatePaymentWorkflow;
using RegisterMe.Application.Exhibitions.Queries.GetPaymentsByExhibitionId;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Domain.Common;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.UpdatePaymentWorkflow;

#region

using static Testing;

#endregion

public class UpdatePaymentWorkflowSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldUpdatePaymentWorkflow()
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

        List<Workflow>? workflow = await DeleteWorkflowSuccessTestWorkflowTestHelper.ReadWorkflowFromFile();
        workflow.Should().NotBeNull();
        UpdatePaymentWorkflowCommand workflowCommandCommand = new()
        {
            PaymentWorkflow = workflow!.First(), ExhibitionId = exhibitionId.Value
        };

        // Act
        await SendAsync(workflowCommandCommand);


        // Assert
        Workflow getPaymentsByExhibitionId =
            await SendAsync(new GetPaymentsByExhibitionIdQuery { ExhibitionId = exhibitionId.Value });
        getPaymentsByExhibitionId.Should().NotBeNull();
        getPaymentsByExhibitionId.Rules.Count().Should().Be(6);
        getPaymentsByExhibitionId.Rules.Select(x => x.Expression).Distinct().Should().Equal("true");
    }
}
