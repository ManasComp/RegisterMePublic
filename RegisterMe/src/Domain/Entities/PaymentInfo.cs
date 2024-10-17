#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class PaymentInfo : BaseAuditableEntity
{
    public DateTimeOffset? PaymentRequestDate { get; init; }
    public DateTimeOffset? PaymentCompletedDate { get; set; }
    public string? PaymentIntentId { get; set; }
    public decimal? Amount { get; set; }
    public PaymentType? PaymentType { get; set; }
    public Currency? Currency { get; init; }
    public string? SessionId { get; set; }
    public int RegistrationToExhibitionId { get; init; }

    [ForeignKey(nameof(RegistrationToExhibitionId))]
    public virtual RegistrationToExhibition RegistrationToExhibition { get; init; } = null!;
}
