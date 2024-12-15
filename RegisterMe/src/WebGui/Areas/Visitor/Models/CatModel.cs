#region

using System.ComponentModel.DataAnnotations;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.CatRegistrations.Enums;
using RegisterMe.Domain.Enums;

#endregion

namespace WebGui.Areas.Visitor.Models;

public sealed class CatModel : EmsModel
{
    public required Gender Gender { get; init; }

    [Required] public string Name { get; init; } = null!;

    [Required] public string PedigreeNumber { get; init; } = null!;

    public string? TitleBeforeName { get; init; }
    public string? TitleAfterName { get; init; }

    public static CatModel InitializeFrom(ParentDto parentDto,
        int registrationToExhibitionId, bool disabled, RegistrationType registrationType,
        StepInRegistration stepInRegistration)
    {
        return new CatModel
        {
            RegistrationToExhibitionId = registrationToExhibitionId,
            Name = parentDto.Name,
            Ems = parentDto.Ems,
            PedigreeNumber = parentDto.PedigreeNumber,
            Colour = parentDto.Colour,
            Breed = parentDto.Breed,
            TitleAfterName = parentDto.TitleAfterName,
            TitleBeforeName = parentDto.TitleBeforeName,
            Gender = parentDto.Sex,
            Disabled = disabled,
            RegistrationType = registrationType,
            Step = (int)stepInRegistration
        };
    }

    public static MotherDto ConvertToMother(CatModel model)
    {
        if (model.Gender != Gender.Female)
        {
            throw new ArgumentException();
        }

        return new MotherDto
        {
            TitleBeforeName = model.TitleBeforeName,
            TitleAfterName = model.TitleAfterName,
            Name = model.Name,
            Ems = model.Ems,
            PedigreeNumber = model.PedigreeNumber,
            Colour = model.Colour,
            Breed = model.Breed
        };
    }

    public static FatherDto ConvertToFather(CatModel model)
    {
        if (model.Gender != Gender.Male)
        {
            throw new ArgumentException();
        }

        return new FatherDto
        {
            TitleBeforeName = model.TitleBeforeName,
            TitleAfterName = model.TitleAfterName,
            Name = model.Name,
            Ems = model.Ems,
            PedigreeNumber = model.PedigreeNumber,
            Colour = model.Colour,
            Breed = model.Breed
        };
    }
}
