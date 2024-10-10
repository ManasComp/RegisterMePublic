#region

using RegisterMe.Application.CatRegistrations.Dtos;

#endregion

namespace WebApi.Dtos;

public record CreteLitterRequest
{
    public required int RegistrationToExhibitionId { get; init; }
    public required List<CreateCatDayDto> CatDay { get; init; } = [];
    public required string? Note { get; init; }
    public required LitterDto Litter { get; init; } = null!;
}
