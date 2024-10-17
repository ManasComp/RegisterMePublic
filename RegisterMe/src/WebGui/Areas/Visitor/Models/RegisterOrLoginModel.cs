namespace WebGui.Areas.Visitor.Models;

public class RegisterOrLoginModel
{
    public required string Name { get; init; } = null!;
    public required string? ReturnUrl { get; init; }
}
