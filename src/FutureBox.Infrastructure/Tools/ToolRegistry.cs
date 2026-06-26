using FutureBox.Domain.Interfaces;

namespace FutureBox.Infrastructure.Tools;

public class ToolRegistry(IEnumerable<ITool> tools) : IToolRegistry
{
    private readonly IReadOnlyList<ITool> _tools = tools.ToList();

    public TInterface Get<TInterface>() where TInterface : ITool
        => _tools.OfType<TInterface>().FirstOrDefault()
           ?? throw new InvalidOperationException($"Tool '{typeof(TInterface).Name}' is not registered.");

    public bool IsAvailable<TInterface>() where TInterface : ITool
        => _tools.OfType<TInterface>().Any();

    public IReadOnlyList<ITool> GetAll() => _tools;
}
