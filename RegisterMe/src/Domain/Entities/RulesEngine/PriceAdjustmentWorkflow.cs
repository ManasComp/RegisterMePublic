#region

using System.ComponentModel.DataAnnotations.Schema;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Domain.Entities.RulesEngine;

public class PriceAdjustmentWorkflow : AbstractWorkflow
{
    public PriceAdjustmentWorkflow()
    {
    }

    public PriceAdjustmentWorkflow(Workflow w, int exhibitionId) : base(w)
    {
        ExhibitionId = exhibitionId;
    }

    public int ExhibitionId { get; init; }

    [ForeignKey(nameof(ExhibitionId))] public virtual Exhibition Exhibition { get; init; } = null!;
}
