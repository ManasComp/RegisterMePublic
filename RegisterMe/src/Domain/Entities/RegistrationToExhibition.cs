#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class RegistrationToExhibition : BaseAuditableEntity
{
    public required int ExhibitionId { get; init; }
    public required int ExhibitorId { get; init; }
    [ForeignKey(nameof(ExhibitionId))] public virtual Exhibition Exhibition { get; init; } = null!;
    public virtual required PersonRegistration PersonRegistration { get; init; } = null!;
    public virtual ICollection<CatRegistration> CatRegistrations { get; init; } = new List<CatRegistration>();
    [ForeignKey(nameof(ExhibitorId))] public virtual Exhibitor Exhibitor { get; init; } = null!;
    public required int AdvertisementId { get; set; }
    [ForeignKey(nameof(AdvertisementId))] public virtual Advertisement Advertisement { get; init; } = null!;
    public virtual PaymentInfo? PaymentInfo { get; set; }
    public virtual List<PersonCage> Cages { get; init; } = [];

    public DateTimeOffset? LastNotificationSendOn { get; set; }
}
