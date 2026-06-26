namespace FutureBox.Domain.Interfaces;

/// <summary>
/// Provides agents with access to registered tools.
/// </summary>
public interface IToolRegistry
{
    /// <summary>Returns the tool implementation for the given interface type.</summary>
    TInterface Get<TInterface>() where TInterface : ITool;

    /// <summary>Returns true if the given tool type is registered and available.</summary>
    bool IsAvailable<TInterface>() where TInterface : ITool;

    /// <summary>Returns all registered tools.</summary>
    IReadOnlyList<ITool> GetAll();
}
