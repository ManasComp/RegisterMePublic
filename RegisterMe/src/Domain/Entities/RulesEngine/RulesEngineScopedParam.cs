#region

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Domain.Entities.RulesEngine;

/// <summary>
///     Class LocalParam.
/// </summary>
[ExcludeFromCodeCoverage]
public class RulesEngineScopedParam
{
    public RulesEngineScopedParam()
    {
    }

    public RulesEngineScopedParam(ScopedParam p)
    {
        Name = p.Name;
        Expression = p.Expression;
    }

    /// <summary>
    ///     Gets or sets the name of the param.
    /// </summary>
    /// <value>
    ///     The name of the rule.
    /// </value>
    /// ]
    public string Name { get; init; } = null!;

    /// <summary>
    ///     Gets or Sets the lambda expression which can be reference in Rule.
    /// </summary>
    public string Expression { get; init; } = null!;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    public ScopedParam GetAsScopedParam()
    {
        ScopedParam p = new() { Name = Name, Expression = Expression };
        return p;
    }
}
