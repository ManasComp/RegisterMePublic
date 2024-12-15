namespace RegisterMe.Application.Cages.Dtos;

public record CagesRec
{
    public required int NumberOfSingleCages { get; set; }
    public required int NumberOfDoubleCages { get; set; }
    public decimal TotalNumber => NumberOfDoubleCages + (NumberOfSingleCages / (decimal)2);
}
