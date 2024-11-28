#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class PaymentInfo : BaseAuditableEntity
{
    public DateTimeOffset PaymentRequestDate { get; set; }
    public DateTimeOffset? PaymentCompletedDate { get; set; }
    public string? PaymentIntentId { get; set; }
    public PaymentType PaymentType { get; set; }
    public string? SessionId { get; set; }
    public int RegistrationToExhibitionId { get; init; }

    [ForeignKey(nameof(RegistrationToExhibitionId))]
    public virtual RegistrationToExhibition RegistrationToExhibition { get; init; } = null!;

    public virtual Amounts Amounts { get; set; } = null!;
}
