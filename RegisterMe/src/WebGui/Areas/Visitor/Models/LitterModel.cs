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

public class LitterModel : StepModel
{
    public required string? PassOfOrigin { get; init; }

    [Required] public required string Breed { get; init; } = null!;

    [Required]
    [DateOnlyRange("1900-01-01", ErrorMessage = "Date of Birth must be between 1900-01-01 and 2100-01-01")]
    public required DateOnly DateOfBirth { get; init; }

    [Required] public required bool IsSameAsExhibitor { get; init; }

    [Required] public required string NameOfBreedingStation { get; init; } = null!;

    [Remote(areaName: "Visitor", controller: "Registration", action: "VerifyBreederName",
        AdditionalFields = nameof(IsSameAsExhibitor))]
    public required string? BreederName { get; set; }

    [Remote(areaName: "Visitor", controller: "Registration", action: "VerifyBreederSurname",
        AdditionalFields = nameof(IsSameAsExhibitor))]
    public required string? BreederSurname { get; set; }

    [Remote(areaName: "Visitor", controller: "Registration", action: "VerifyBreederCountry",
        AdditionalFields = nameof(IsSameAsExhibitor))]
    public required string? BreederCountry { get; set; }

    [ValidateNever] public required IEnumerable<SelectListItem> Countries { get; set; }


    public static LitterModel InitializeFrom(
        CreateLitterDto catDto, int registrationToExhibitionId, bool disabled,
        StepInRegistration stepInRegistration, List<SelectListItem> countries)
    {
        return new LitterModel
        {
            Countries = countries,
            Disabled = disabled,
            RegistrationType = RegistrationType.Litter,
            Step = (int)StepInRegistration.StartedExhibitedCatAndBreederInit,
            NameOfBreedingStation = catDto.NameOfBreedingStation,
            PassOfOrigin = catDto.PassOfOrigin,
            DateOfBirth = catDto.BirthDate,
            BreederName = catDto.Breeder.FirstName,
            BreederSurname = catDto.Breeder.LastName,
            BreederCountry = catDto.Breeder.Country,
            Breed = catDto.Breed,
            RegistrationToExhibitionId = registrationToExhibitionId,
            IsSameAsExhibitor = catDto.Breeder.BreederIsSameAsExhibitor
        };
    }

    public TemporaryCatRegistrationDto GetAsCatRegistrationTemporary()
    {
        return new TemporaryCatRegistrationDto
        {
            Id = null,
            StepInRegistration = StepInRegistration.FinishedExhibitedCatAndLitterAndBreederInit,
            CatDays = [], // this is correct as we do not have information yet
            ExhibitedCat = null,
            Litter = new CreateLitterDto
            {
                NameOfBreedingStation = NameOfBreedingStation,
                BirthDate = DateOfBirth,
                Breed = Breed,
                Breeder = new BreederDto
                {
                    FirstName = BreederName!,
                    LastName = BreederSurname!,
                    Country = BreederCountry!,
                    BreederIsSameAsExhibitor = IsSameAsExhibitor
                },
                Father = null!, // this is correct as we do not have information yet
                Mother = null!, // this is correct as we do not have information yet
                PassOfOrigin = PassOfOrigin
            },
            Note = null, // this is correct as we do not have information yet
            RegistrationToExhibitionId = RegistrationToExhibitionId
        };
    }

    public static LitterModel GetInitializedLitterModel(int registrationToExhibitionId, bool disabled,
        List<SelectListItem> countries)
    {
        return new LitterModel
        {
            Countries = countries,
            Step = (int)StepInRegistration.StartedExhibitedCatAndBreederInit,
            RegistrationType = RegistrationType.Litter,
            Disabled = disabled,
            NameOfBreedingStation = "",
            PassOfOrigin = null,
            DateOfBirth = new DateOnly(DateTime.Now.AddMonths(-7).Year, DateTime.Now.AddMonths(-7).Month, 20),
            BreederName = "",
            BreederSurname = "",
            BreederCountry = "",
            Breed = "",
            RegistrationToExhibitionId = registrationToExhibitionId,
            IsSameAsExhibitor = true
        };
    }
}
