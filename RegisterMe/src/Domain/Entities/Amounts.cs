namespace RegisterMe.Domain.Entities;

public class Amounts : BaseAuditableEntity
{
    public required Currency Currency { get; init; }
    public required decimal Amount { get; set; }

    public int? PriceId { get; init; }
    public virtual Price? Price { get; init; }

    public int? PaymentInfoId { get; init; }
    public virtual PaymentInfo? PaymentInfo { get; init; }

    public int? AdvertisementId { get; init; }
    public virtual Advertisement? Advertisement { get; init; }
}
