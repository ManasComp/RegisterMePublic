#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace RegisterMe.Domain.Entities;

public class Litter : BaseEntity
{
    public required string? PassOfOrigin { get; init; }
    public required string NameOfBreedingStation { get; init; } = null!;
    public required DateOnly BirthDate { get; init; }
    public required string Breed { get; init; } = null!;
    public virtual Parent Father { get; init; } = null!;
    public virtual Parent Mother { get; init; } = null!;
    public virtual Breeder Breeder { get; init; } = null!;

    [ForeignKey(nameof(CatRegistrationId))]
    public virtual CatRegistration CatRegistration { get; init; } = null!;

    public required int CatRegistrationId { get; init; }
}
