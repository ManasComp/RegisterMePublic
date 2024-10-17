namespace RegisterMe.Application.CatRegistrations.Dtos;

public record TemporaryExhibitedCatDto : CreateExhibitedCatDto
{
    public required bool IsHomeCat { get; set; }
}
