#region

using RulesEngine.Models;

#endregion

namespace RegisterMe.Domain.Entities.RulesEngine;

public class WorkflowDto : Workflow
{
    public int Id { get; init; }

    public Workflow ToWorkflow()
    {
        return this;
    }
}
