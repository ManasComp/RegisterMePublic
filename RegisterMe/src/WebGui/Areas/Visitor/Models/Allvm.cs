namespace WebGui.Areas.Visitor.Models;

public record AllVm
{
    public required ExhibitedCatModel? ExhibitedCatModel { get; init; }
    public required LitterModel? LitterModel { get; init; }
    public required CatModel? FatherModel { get; init; }
    public required CatModel? MotherModel { get; init; }
    public required MultipleExhibitionVm ExhibitionVm { get; init; } = null!;
    public required int CatRegistrationId { get; init; }
    public required int RegistrationToExhibitionId { get; init; }
    public required bool CanBeEdited { get; init; }
}
