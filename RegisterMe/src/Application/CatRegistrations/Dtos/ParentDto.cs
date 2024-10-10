#region

using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.CatRegistrations.Dtos;

public abstract record ParentDto
{
    public string? TitleBeforeName { get; init; }
    public string? TitleAfterName { get; init; }
    public string Name { get; init; } = null!;
    public string Ems { get; init; } = null!;
    public string PedigreeNumber { get; init; } = null!;
    public string? Colour { get; init; }
    public string Breed { get; init; } = null!;
    public abstract Gender Sex { get; }
    public string FullName => $"{TitleBeforeName} {Name} {TitleAfterName}".Trim();
}
