#region

using RulesEngine.Models;

#endregion

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable MemberCanBePrivate.Global

namespace RegisterMe.Domain.Entities.RulesEngine;

public abstract class AbstractWorkflow : BaseEntity
{
    protected AbstractWorkflow()
    {
    }

    protected AbstractWorkflow(Workflow w)
    {
        WorkflowName = w.WorkflowName;
        RuleExpressionType = w.RuleExpressionType;
        GlobalParams = w.GlobalParams?.Select(x => new RulesEngineScopedParam(x)) ?? new List<RulesEngineScopedParam>();
        Rules = w.Rules?.Select(x => new RulesEngineRule(x)).ToList() ?? [];
    }

    /// <summary>
    ///     Gets the workflow name.
    /// </summary>
    public string WorkflowName { get; set; } = null!;

    public RuleExpressionType RuleExpressionType { get; set; } = RuleExpressionType.LambdaExpression;

    /// <summary>
    ///     Gets or Sets the global params which will be applicable to all rules
    /// </summary>
    public virtual IEnumerable<RulesEngineScopedParam> GlobalParams { get; set; } = null!;

    /// <summary>
    ///     List of rules.
    /// </summary>
    public virtual List<RulesEngineRule> Rules { get; set; } = null!;

    public Workflow GetAsWorkflow()
    {
        Workflow w = new()
        {
            WorkflowName = WorkflowName,
            RuleExpressionType = RuleExpressionType,
            GlobalParams = GlobalParams.Select(x => x.GetAsScopedParam()),
            Rules = Rules.Select(x => x.GetAsRule())
        };
        return w;
    }

    public WorkflowDto GetAsWorkflowDto()
    {
        WorkflowDto w = new()
        {
            Id = Id,
            WorkflowName = WorkflowName,
            RuleExpressionType = RuleExpressionType,
            GlobalParams = GlobalParams.Select(x => x.GetAsScopedParam()).ToList(),
            Rules = Rules.Select(x => x.GetAsRule()).ToList()
        };
        return w;
    }
}
