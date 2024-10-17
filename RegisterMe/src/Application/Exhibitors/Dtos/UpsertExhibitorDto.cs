#region

using Microsoft.AspNetCore.Identity;

#endregion

namespace RegisterMe.Application.Exhibitors.Dtos;

public class UpsertExhibitorDto
{
    [ProtectedPersonalData] public required string Organization { get; init; } = null!;
    [ProtectedPersonalData] public required string MemberNumber { get; init; } = null!;
    [ProtectedPersonalData] public required string Country { get; init; } = null!;
    [ProtectedPersonalData] public required string City { get; init; } = null!;
    [ProtectedPersonalData] public required string Street { get; init; } = null!;
    [ProtectedPersonalData] public required string HouseNumber { get; init; } = null!;
    [ProtectedPersonalData] public required string ZipCode { get; init; } = null!;
    public required bool IsPartOfCsch { get; init; }
    [ProtectedPersonalData] public required string EmailToOrganization { get; init; } = null!;
    public required bool IsPartOfFife { get; init; }
}
