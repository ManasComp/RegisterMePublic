#region

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

#endregion

namespace RegisterMe.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    [ProtectedPersonalData] public DateOnly DateOfBirth { get; set; }
    [ProtectedPersonalData] public string FirstName { get; set; } = null!;
    [ProtectedPersonalData] public string LastName { get; set; } = null!;
    public int? OrganizationId { get; init; }
    [ForeignKey(nameof(OrganizationId))] public virtual Organization? OrganizationUserIsAdminIn { get; init; }
    public virtual Exhibitor? Exhibitor { get; init; }

    public void CopyValuesTo(ApplicationUser other)
    {
        other.FirstName = FirstName;
        other.LastName = LastName;
        other.DateOfBirth = DateOfBirth;
        other.PhoneNumber = PhoneNumber;
    }
}
