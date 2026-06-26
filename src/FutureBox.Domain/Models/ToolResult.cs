namespace FutureBox.Domain.Models;

public record ToolResult
{
    public required bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public IReadOnlyDictionary<string, object> Data { get; init; } = new Dictionary<string, object>();

    public static ToolResult Success(IReadOnlyDictionary<string, object>? data = null) =>
        new() { IsSuccess = true, Data = data ?? new Dictionary<string, object>() };

    public static ToolResult Failure(string errorMessage) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage };
}
