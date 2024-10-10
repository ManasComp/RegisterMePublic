#region

using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.CatRegistrations.Enums;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.CatRegistrations.Dtos;

public class TemporaryCatRegistrationDto
{
    public required TemporaryExhibitedCatDto? ExhibitedCat { get; init; }
    public required CreateLitterDto? Litter { get; init; }
    public required string? Note { get; set; }
    public required List<TemporaryCatDayDto> CatDays { get; set; } = [];
    public required int RegistrationToExhibitionId { get; init; }

    public required StepInRegistration StepInRegistration { get; set; } =
        StepInRegistration.FinishedExhibitedCatAndLitterAndBreederInit;

    public required int? Id { get; init; }

    public RegistrationType RegistrationType => this switch
    {
        { ExhibitedCat: null } => RegistrationType.Litter,
        { ExhibitedCat.IsHomeCat: true } => RegistrationType.HomeExhibitedCat,
        _ => RegistrationType.NonHomeExhibitedCat
    };

    public static TemporaryCatRegistrationDto InitializeFrom(CatRegistrationDto catRegistrationDto)
    {
        return new TemporaryCatRegistrationDto
        {
            Litter = catRegistrationDto.Litter,
            CatDays =
            [
                ..catRegistrationDto.CatDays.Select(x =>
                    new TemporaryCatDayDto
                    {
                        ExhibitionDayId = x.ExhibitionDayId,
                        GroupsIds = x.GroupsIds,
                        RentedCageTypeId = x.RentedCageTypeId,
                        Cage = new PersonCageDto { CreateCage = null, PersonCageId = x.ExhibitorsCage }
                    })
            ],
            ExhibitedCat =
                catRegistrationDto.ExhibitedCat == null
                    ? null
                    : new TemporaryExhibitedCatDto
                    {
                        BirthDate = catRegistrationDto.ExhibitedCat.BirthDate,
                        Breed = catRegistrationDto.ExhibitedCat.Breed,
                        Breeder = catRegistrationDto.ExhibitedCat.Breeder,
                        Colour = catRegistrationDto.ExhibitedCat.Colour,
                        Ems = catRegistrationDto.ExhibitedCat.Ems,
                        Father = catRegistrationDto.ExhibitedCat.Father,
                        Group = catRegistrationDto.ExhibitedCat.Group,
                        Mother = catRegistrationDto.ExhibitedCat.Mother,
                        Name = catRegistrationDto.ExhibitedCat.Name,
                        Neutered = catRegistrationDto.ExhibitedCat.Neutered,
                        PedigreeNumber = catRegistrationDto.ExhibitedCat.PedigreeNumber,
                        TitleAfterName = catRegistrationDto.ExhibitedCat.TitleAfterName,
                        TitleBeforeName = catRegistrationDto.ExhibitedCat.TitleBeforeName,
                        Sex = catRegistrationDto.ExhibitedCat.Sex,
                        IsHomeCat =
                            catRegistrationDto.ExhibitedCat.Father == null &&
                            catRegistrationDto.ExhibitedCat.Mother == null
                    },
            Id = catRegistrationDto.Id,
            Note = catRegistrationDto.Note,
            RegistrationToExhibitionId = catRegistrationDto.RegistrationToExhibitionId,
            StepInRegistration = StepInRegistration.StartedExhibitedCatAndBreederInit
        };
    }
}
