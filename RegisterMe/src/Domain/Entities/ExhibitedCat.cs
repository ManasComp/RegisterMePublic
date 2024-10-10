#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class ExhibitedCat : BaseEntity
{
    public required Gender Sex { get; init; } = Gender.Female;
    public required bool Neutered { get; init; }
    public required string? TitleBeforeName { get; init; }
    public required string? TitleAfterName { get; init; }
    public required string Name { get; init; } = null!;
    public required string Ems { get; init; } = null!;
    public required string? PedigreeNumber { get; init; }
    public required string? Colour { get; init; }
    public required int? Group { get; init; }
    public required DateOnly BirthDate { get; init; }
    public required string Breed { get; init; } = null!;
    public virtual Parent? Father { get; init; }
    public virtual Parent? Mother { get; init; }
    public virtual Breeder? Breeder { get; init; }

    [ForeignKey(nameof(CatRegistrationId))]
    public virtual CatRegistration CatRegistration { get; init; } = null!;

    public required int CatRegistrationId { get; init; }
}
