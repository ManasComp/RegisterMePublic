#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class PersonRegistration : BaseEntity
{
    public required string City { get; init; } = null!;
    public required string Street { get; init; } = null!;
    public required string HouseNumber { get; init; } = null!;
    public required string ZipCode { get; init; } = null!;
    public required string? Organization { get; init; }
    public required string MemberNumber { get; init; } = null!;
    public required string FirstName { get; init; } = null!;
    public required string LastName { get; init; } = null!;
    public required string Country { get; init; } = null!;
    public required DateOnly DateOfBirth { get; init; }
    public required string Email { get; init; } = null!;
    public required string PhoneNumber { get; init; } = null!;
    public int RegistrationToExhibitionId { get; init; }
    public required bool IsPartOfCsch { get; init; }
    public required bool IsPartOfFife { get; init; }
    public string EmailToOrganization { get; init; } = null!;

    [ForeignKey(nameof(RegistrationToExhibitionId))]
    public virtual RegistrationToExhibition RegistrationToExhibition { get; init; } = null!;
}
