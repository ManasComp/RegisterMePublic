namespace RegisterMe.Application.Cages.Dtos;

public record CatRegistrationStatistics
{
    public required int OwnCages { get; init; }
    public required CagesRec RentedCages { get; init; }
    public required int NumberOfCats { get; init; }
    public required DateOnly Date { get; init; }
}
