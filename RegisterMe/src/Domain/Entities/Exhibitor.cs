#region

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

#endregion

namespace RegisterMe.Domain.Entities;

public class Exhibitor : BaseAuditableEntity
{
    [ProtectedPersonalData] public required string Country { get; set; } = null!;
    [ProtectedPersonalData] public required string City { get; set; } = null!;
    [ProtectedPersonalData] public required string Street { get; set; } = null!;
    [ProtectedPersonalData] public required string HouseNumber { get; set; } = null!;
    [ProtectedPersonalData] public required string ZipCode { get; set; } = null!;
    [ProtectedPersonalData] public required string Organization { get; set; } = null!;
    [ProtectedPersonalData] public required string MemberNumber { get; set; } = null!;
    public virtual List<RegistrationToExhibition> RegistrationToExhibitions { get; init; } = [];
    public required string AspNetUserId { get; init; } = null!;
    [ForeignKey(nameof(AspNetUserId))] public virtual ApplicationUser AspNetUser { get; init; } = null!;
    public required bool IsPartOfCsch { get; set; }
    public required bool IsPartOfFife { get; set; }
    [ProtectedPersonalData] public required string EmailToOrganization { get; set; } = null!;
}
