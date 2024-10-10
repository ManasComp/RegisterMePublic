#region

using Newtonsoft.Json;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.CreateWorkflowCommand;

public class WorkflowTestHelper
{
    public static async Task<List<Workflow>?> ReadWorkflowFromFile(string fileName = "CatRegistrationWorkflow.json")
    {
        string currentDir = Directory.GetCurrentDirectory();
        string desiredDir =
            Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDir)!.FullName)!.FullName)!.FullName;
        string[] files = Directory.GetFiles(desiredDir, fileName, SearchOption.AllDirectories);
        files.Length.Should().BeGreaterThan(0);

        string fileData = await File.ReadAllTextAsync(files[0]);
        List<Workflow>? workflow = JsonConvert.DeserializeObject<List<Workflow>>(fileData);
        workflow.Should().NotBeNull();

        return workflow;
    }
}
