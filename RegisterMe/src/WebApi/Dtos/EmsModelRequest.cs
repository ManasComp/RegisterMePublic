namespace WebApi.Dtos;

public class EmsModelRequest
{
    public required string Ems { get; set; } = null!;
    public required string Breed { get; set; } = null!;
    public required string Colour { get; set; } = null!;
}
