namespace RegisterMe.Application.CatRegistrations.Dtos;

public record CreateCatRegistrationDto : AbstractCatRegistrationDto
{
    public required int RegistrationToExhibitionId { get; init; }
    public required List<CreateCatDayDto> CatDays { get; init; } = [];
}
