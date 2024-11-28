#region

using System.ComponentModel.DataAnnotations.Schema;
using RegisterMe.Domain.Entities.RulesEngine;

#endregion

namespace RegisterMe.Domain.Entities;

public class Exhibition : BaseAuditableEntity
{
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsPublished { get; set; }
    public string Url { get; set; } = null!;
    public string BankAccount { get; set; } = null!;
    public string Iban { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateOnly RegistrationStart { get; set; }
    public DateOnly RegistrationEnd { get; set; }
    public int OrganizationId { get; set; }
    public bool IsCancelled { get; set; }
    public int DeleteNotFinishedRegistrationsAfterHours { get; set; }
    public virtual Address Address { get; set; } = null!;
    public virtual ICollection<ExhibitionDay> Days { get; init; } = new List<ExhibitionDay>();
    public virtual ICollection<Advertisement> Advertisements { get; init; } = new List<Advertisement>();
    public virtual ICollection<PriceAdjustmentWorkflow> Workflows { get; init; } = new List<PriceAdjustmentWorkflow>();
    public virtual PriceTypeWorkflow PaymentTypesWorkflow { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the registrations to exhibitions.
    /// </summary>
    public virtual ICollection<RegistrationToExhibition> RegistrationsToExhibitions { get; } =
        new List<RegistrationToExhibition>();

    /// <summary>
    ///     Gets or sets the organization.
    /// </summary>
    [ForeignKey(nameof(OrganizationId))]
    public virtual Organization Organization { get; init; } = null!;

    public static Exhibition CreateInstance()
    {
        return new Exhibition();
    }
}
