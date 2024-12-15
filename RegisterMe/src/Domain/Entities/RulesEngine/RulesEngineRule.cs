#region

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RulesEngine.Models;

#endregion

// ReSharper disable VirtualMemberCallInConstructor

namespace RegisterMe.Domain.Entities.RulesEngine;

#nullable disable
public class RulesEngineRule
{
    public RulesEngineRule()
    {
    }

    public RulesEngineRule(Rule r)
    {
        RuleName = r.RuleName;
        Properties = r.Properties;
        Operator = r.Operator;
        ErrorMessage = r.ErrorMessage;
        Enabled = r.Enabled;
        RuleExpressionType = r.RuleExpressionType;
        WorkflowsToInject = r.WorkflowsToInject;
        LocalParams = r.LocalParams?.Select(x => new RulesEngineScopedParam(x));
        Expression = r.Expression;
        Actions = r.Actions;
        SuccessEvent = r.SuccessEvent;
        Rules = r.Rules?.Select(x => new RulesEngineRule(x)).ToList();
    }

    public virtual List<RulesEngineRule> Rules { get; init; }
    public string RuleName { get; init; }
    public virtual Dictionary<string, object> Properties { get; init; }
    public string Operator { get; init; }
    public string ErrorMessage { get; init; }
    public bool Enabled { get; init; } = true;

    [JsonConverter(typeof(StringEnumConverter))]
    public RuleExpressionType RuleExpressionType { get; init; }

    public virtual IEnumerable<string> WorkflowsToInject { get; init; }
    public virtual IEnumerable<RulesEngineScopedParam> LocalParams { get; init; }
    public string Expression { get; init; }
    public RuleActions Actions { get; init; }
    public string SuccessEvent { get; init; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    public Rule GetAsRule()
    {
        Rule r = new()
        {
            RuleName = RuleName,
            Properties = Properties,
            Operator = Operator,
            ErrorMessage = ErrorMessage,
            Enabled = Enabled,
            RuleExpressionType = RuleExpressionType,
            WorkflowsToInject = WorkflowsToInject,
            LocalParams = LocalParams?.Select(x => x.GetAsScopedParam()),
            Expression = Expression,
            Actions = Actions,
            SuccessEvent = SuccessEvent,
            Rules = Rules?.Select(x => x.GetAsRule())
        };
        return r;
    }
}
