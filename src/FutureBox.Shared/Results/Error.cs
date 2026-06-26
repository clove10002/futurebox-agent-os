namespace FutureBox.Shared.Results;

public record Error(string Code, string Message)
{
    public static Error NotFound(string message) => new("NOT_FOUND", message);
    public static Error Validation(string message) => new("VALIDATION", message);
    public static Error Unexpected(string message) => new("UNEXPECTED", message);
    public static Error Cancelled(string message) => new("CANCELLED", message);
    public static Error ToolUnavailable(string tool) => new("TOOL_UNAVAILABLE", $"Tool '{tool}' is not available.");
}
