using FutureBox.Domain.Interfaces;
using System.Collections.Concurrent;

namespace FutureBox.Infrastructure.Memory;

public class InMemoryMemoryStore : IMemoryStore
{
    private readonly ConcurrentDictionary<string, object> _store = new();

    public Task SetAsync<T>(string key, T value, CancellationToken ct)
    {
        if (value is not null)
            _store[key] = value;
        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        if (_store.TryGetValue(key, out var raw) && raw is T typed)
            return Task.FromResult<T?>(typed);
        return Task.FromResult<T?>(default);
    }

    public Task<bool> ExistsAsync(string key, CancellationToken ct)
        => Task.FromResult(_store.ContainsKey(key));

    public Task RemoveAsync(string key, CancellationToken ct)
    {
        _store.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}
