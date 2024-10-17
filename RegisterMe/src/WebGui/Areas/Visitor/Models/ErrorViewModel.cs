namespace WebGui.Areas.Visitor.Models;

public class ErrorViewModel
{
    public required string? RequestId { get; init; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public required string? ExceptionMessage { get; init; }
}
