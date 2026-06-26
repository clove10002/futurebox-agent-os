namespace FutureBox.Domain.Models;

public abstract record ToolRequest
{
    public Guid RequestId { get; init; } = Guid.NewGuid();
}
