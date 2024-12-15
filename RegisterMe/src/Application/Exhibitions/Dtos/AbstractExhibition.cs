// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace RegisterMe.Application.Exhibitions.Dtos;

public abstract record AbstractExhibitionDto
{
    public required string Name { get; init; } = null!;
    public required string Url { get; init; } = null!;
    public required string Description { get; init; } = null!;
    public required string BankAccount { get; init; } = null!;
    public required string Iban { get; init; } = null!;
    public required bool IsPublished { get; init; }
    public required bool IsCancelled { get; init; }
    public required string Phone { get; init; } = null!;
    public required string Email { get; init; } = null!;
    public required DateOnly RegistrationStart { get; init; }
    public required DateOnly RegistrationEnd { get; init; }
    public required DateOnly ExhibitionStart { get; init; }
    public required DateOnly ExhibitionEnd { get; init; }
    public required int OrganizationId { get; init; }
    public bool CanBeEdited => !IsPublished && !IsCancelled;
    public required int DeleteNotFinishedRegistrationsAfterHours { get; init; }
}
