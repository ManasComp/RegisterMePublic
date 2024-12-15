#region

using RegisterMe.Application.CatRegistrations.Enums;

#endregion

namespace RegisterMe.Application.CatRegistrations.Dtos;

public abstract record AbstractCatRegistrationDto
{
    public required CreateExhibitedCatDto? ExhibitedCat { get; init; }
    public required CreateLitterDto? Litter { get; init; }
    public required string? Note { get; init; }

    public RegistrationType RegistrationType => this switch
    {
        { ExhibitedCat: null } => RegistrationType.Litter,
        { ExhibitedCat: { Mother: null, Father: null } } => RegistrationType.HomeExhibitedCat,
        _ => RegistrationType.NonHomeExhibitedCat
    };
}
