namespace FutureBox.Domain.Interfaces;

/// <summary>
/// Provides scoped, keyed storage for intermediate agent results within a workflow.
/// </summary>
public interface IMemoryStore
{
    Task SetAsync<T>(string key, T value, CancellationToken ct);
    Task<T?> GetAsync<T>(string key, CancellationToken ct);
    Task<bool> ExistsAsync(string key, CancellationToken ct);
    Task RemoveAsync(string key, CancellationToken ct);
}
