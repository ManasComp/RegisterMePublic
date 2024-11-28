#region

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.CatRegistrations.Enums;
using RegisterMe.Domain.Enums;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class ExhibitedCatModel : EmsModel
{
    [Required] public required string Name { get; init; } = null!;

    [Remote(areaName: "Visitor", controller: "Registration", action: "VerifyBreedingBook",
        AdditionalFields = nameof(DateOfBirth))]
    public required string? BreedingBook { get; init; }

    [Required]
    [DateOnlyRange("1900-01-01", ErrorMessage = "Datum musí být větší než 1900-01-01 a menší než aktuální datum")]
    public required DateOnly DateOfBirth { get; init; } = DateOnly.FromDateTime(DateTime.Now);

    [Required] public required bool Castrated { get; init; }

    [Required] public required bool IsSameAsExhibitor { get; init; }

    [Required] public required Gender Gender { get; init; } = Gender.Female;

    [Remote(areaName: "Visitor", controller: "Registration", action: "VerifyBreederName",
        AdditionalFields = nameof(IsSameAsExhibitor))]
    public required string? BreederName { get; set; }

    [Remote(areaName: "Visitor", controller: "Registration", action: "VerifyBreederSurname",
        AdditionalFields = nameof(IsSameAsExhibitor))]
    public required string? BreederSurname { get; set; }

    [Remote(areaName: "Visitor", controller: "Registration", action: "VerifyBreederCountry",
        AdditionalFields = nameof(IsSameAsExhibitor))]
    public required string? BreederCountry { get; set; }

    public required string? TitleBeforeName { get; init; }
    public required string? TitleAfterName { get; init; }

    [Remote(areaName: "Visitor", controller: "Registration", action: "VerifyGroup",
        AdditionalFields = nameof(Ems))]
    [Range(1, 12, ErrorMessage = "Hodnota musí být mezi 1 a 12")]
    public required int? Group { get; init; }

    public required bool IsHomeCat { get; init; }
    public required bool HasBreeder { get; init; }

    [ValidateNever] public required IEnumerable<SelectListItem> Countries { get; set; }

    public static ExhibitedCatModel InitializeFrom(
        TemporaryExhibitedCatDto catDto, int registrationToExhibitionId, bool disabled,
        StepInRegistration stepInRegistration, List<SelectListItem> countries)
    {
        return new ExhibitedCatModel
        {
            Countries = countries,
            RegistrationType = RegistrationType.NonHomeExhibitedCat,
            Name = catDto.Name,
            Ems = catDto.Ems,
            BreedingBook = catDto.PedigreeNumber,
            DateOfBirth = catDto.BirthDate,
            Castrated = catDto.Neutered,
            Gender = catDto.Sex,
            BreederName = catDto.Breeder?.FirstName,
            BreederSurname = catDto.Breeder?.LastName,
            BreederCountry = catDto.Breeder?.Country,
            IsHomeCat = catDto.IsHomeCat,
            Colour = catDto.Colour,
            Breed = catDto.Breed,
            TitleAfterName = catDto.TitleAfterName,
            TitleBeforeName = catDto.TitleBeforeName,
            IsSameAsExhibitor = catDto.Breeder?.BreederIsSameAsExhibitor ?? false,
            Step = (int)stepInRegistration,
            Group = catDto.Group,
            RegistrationToExhibitionId = registrationToExhibitionId,
            Disabled = disabled,
            HasBreeder = catDto.Breeder != null
        };
    }

    public static ExhibitedCatModel InitializeFrom(
        CreateExhibitedCatDto catDto, int registrationToExhibitionId, bool disabled,
        StepInRegistration stepInRegistration, List<SelectListItem> countries)
    {
        return new ExhibitedCatModel
        {
            Countries = countries,
            RegistrationType = RegistrationType.NonHomeExhibitedCat,
            Name = catDto.Name,
            Ems = catDto.Ems,
            BreedingBook = catDto.PedigreeNumber,
            DateOfBirth = catDto.BirthDate,
            Castrated = catDto.Neutered,
            Gender = catDto.Sex,
            BreederName = catDto.Breeder?.FirstName,
            BreederSurname = catDto.Breeder?.LastName,
            BreederCountry = catDto.Breeder?.Country,
            IsHomeCat = catDto.Father == null && catDto.Mother == null,
            Colour = catDto.Colour,
            Breed = catDto.Breed,
            TitleAfterName = catDto.TitleAfterName,
            TitleBeforeName = catDto.TitleBeforeName,
            IsSameAsExhibitor = catDto.Breeder?.BreederIsSameAsExhibitor ?? false,
            Step = (int)stepInRegistration,
            Group = catDto.Group,
            RegistrationToExhibitionId = registrationToExhibitionId,
            Disabled = disabled,
            HasBreeder = catDto.Breeder != null
        };
    }

    public static ExhibitedCatModel InitializeBlank(
        RegistrationType registrationType, int registrationToExhibitionId, bool disabled,
        StepInRegistration stepInRegistration, List<SelectListItem> countries)
    {
        return new ExhibitedCatModel
        {
            Countries = countries,
            RegistrationType = registrationType,
            Name = "",
            Ems = "",
            BreedingBook = "",
            DateOfBirth = new DateOnly(DateTime.Now.AddYears(-1).Year, DateTime.Now.AddYears(-1).Month, 20),
            Castrated = false,
            Gender = Gender.Female,
            BreederName = "",
            BreederSurname = "",
            BreederCountry = "",
            IsHomeCat = false,
            Colour = "",
            Breed = "",
            TitleAfterName = "",
            TitleBeforeName = "",
            IsSameAsExhibitor = true,
            Step = (int)stepInRegistration,
            Group = null,
            RegistrationToExhibitionId = registrationToExhibitionId,
            Disabled = disabled,
            HasBreeder = true
        };
    }

    public TemporaryCatRegistrationDto GetAsCatRegistrationTemporary()
    {
        return new TemporaryCatRegistrationDto
        {
            Litter = null,
            Id = null,
            StepInRegistration = StepInRegistration.FinishedExhibitedCatAndLitterAndBreederInit,
            CatDays = [], // this is correct as we do not have information yet
            ExhibitedCat = new TemporaryExhibitedCatDto
            {
                Group = Group,
                BirthDate = DateOfBirth,
                Breed = Breed,
                Breeder = !HasBreeder
                    ? null
                    : new BreederDto
                    {
                        FirstName = BreederName!,
                        LastName = BreederSurname!,
                        Country = BreederCountry!,
                        BreederIsSameAsExhibitor = IsSameAsExhibitor
                    },
                Colour = Colour,
                Father = null, // this is correct as we do not have information yet
                Mother = null, // this is correct as we do not have information yet,
                Name = Name,
                Neutered = Castrated,
                TitleBeforeName = TitleBeforeName,
                TitleAfterName = TitleAfterName,
                Sex = Gender,
                Ems = Ems,
                PedigreeNumber = BreedingBook,
                IsHomeCat = IsHomeCat
            },
            Note = null, // this is correct as we do not have information yet
            RegistrationToExhibitionId = RegistrationToExhibitionId
        };
    }
}
