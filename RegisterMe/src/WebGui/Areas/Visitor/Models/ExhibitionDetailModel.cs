#region

using RegisterMe.Application.Exhibitions.Enums;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.Services.Workflows;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class ExhibitionDetailModel
{
    public required ExhibitionStatus Status { get; init; }
    public required string Name { get; init; } = null!;
    public required OrganizationDto OrganizationDto { get; init; } = null!;
    public required string Phone { get; init; } = null!;
    public required string Email { get; init; } = null!;
    public required string Url { get; init; } = null!;
    public required DateOnly ExhibitionStart { get; init; }
    public required DateOnly ExhibitionEnd { get; init; }
    public required DateOnly RegistrationEnd { get; init; }
    public required AddressDto AddressDto { get; init; } = null!;
    public required string? GoogleApiKey { get; init; }
    public required int ExhibitionId { get; init; }
    public required int Id { get; init; }
    public required bool IsExhibitionManager { get; init; }
    public required bool IsRegistered { get; init; }
    public required bool IsCancelled { get; init; }
    public required bool IsPublished { get; init; }
}
