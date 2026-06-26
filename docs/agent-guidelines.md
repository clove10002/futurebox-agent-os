# FutureBox Agent OS — Agent Guidelines

Agents are the specialized workers of FutureBox. They solve problems by decomposing goals into actions, requesting capabilities from Tools, and reporting progress back to the Orchestrator.

---

## Core Principle

**Agents solve problems. Tools perform actions.**

An agent never directly calls an external API, writes to disk, or launches a subprocess.  
It always requests these capabilities through registered Tools.

This separation ensures:
- Agents remain independently testable
- Tools can be swapped without modifying agents
- The system remains composable and extensible

---

## IAgent Interface

Every agent implements `IAgent`:

```csharp
/// <summary>
/// Represents a specialized worker capable of executing a specific type of task.
/// </summary>
public interface IAgent
{
    /// <summary>
    /// Unique name identifying this agent type.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Human-readable description of what this agent does.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Returns true if this agent can handle the given task.
    /// </summary>
    bool CanHandle(AgentTask task);

    /// <summary>
    /// Executes the task and returns a result.
    /// </summary>
    Task<Result<AgentResult>> ExecuteAsync(AgentContext context, CancellationToken ct);
}
```

---

## AgentContext

Every agent receives an `AgentContext` when executing:

```csharp
public record AgentContext
{
    public required Guid ProjectId { get; init; }
    public required Guid TaskId { get; init; }
    public required string Goal { get; init; }
    public required IReadOnlyDictionary<string, object> Inputs { get; init; }
    public required IToolRegistry Tools { get; init; }
    public required IProgressReporter Progress { get; init; }
    public required IMemoryStore Memory { get; init; }
}
```

Agents access tools and report progress exclusively through the `AgentContext`.

---

## Agent Lifecycle

```
1. Discovery      — Agent is registered in DI and discovered by the AgentDispatcher
2. Selection      — Orchestrator/Planner selects agent based on task type
3. Context Setup  — AgentContext is constructed and injected
4. Execution      — Agent.ExecuteAsync() is called
5. Progress       — Agent reports incremental progress via context.Progress
6. Completion     — Agent returns Result<AgentResult> (success or failure)
7. Cleanup        — Any resources are disposed; outputs are recorded by the tracker
```

---

## Responsibilities

Each agent has **one responsibility**. The responsibility is defined by the goal-type it handles.

| Agent | Responsibility |
|---|---|
| `ResearchAgent` | Gather information from sources for a given topic |
| `ScriptAgent` | Transform research into a structured, narration-ready script |
| `NarrationAgent` | Convert script text to audio narration |
| `SubtitleAgent` | Generate subtitle files from audio |
| `AssetAgent` | Gather or generate media assets for the video |
| `VideoAgent` | Compose final video from assets, audio, and subtitles |
| `PlanningAgent` | Decompose a high-level goal into an ordered execution plan |

Agents are never general-purpose. If an agent is doing two unrelated things, split it.

---

## Progress Reporting

Agents must report meaningful progress throughout execution — not only at completion.

```csharp
await context.Progress.ReportAsync(new ProgressUpdate
{
    TaskId = context.TaskId,
    AgentName = Name,
    Message = "Fetching sources from Wikipedia...",
    PercentComplete = 30,
    Timestamp = DateTimeOffset.UtcNow
}, ct);
```

Progress flows through SignalR to the UI. Users should always know what the agent is doing.

Never use `Console.WriteLine` or direct logging for progress — always use `IProgressReporter`.

---

## Error Handling and Recovery

- Agents return `Result<AgentResult>` — they do not throw for expected failures
- Expected failures: rate limits, missing data, empty search results, unsupported formats
- Unexpected failures (bugs, system errors) propagate as exceptions for the Orchestrator to catch
- Agents should retry transient failures (network timeouts) using configurable retry policies
- Agents must not silently swallow errors — every failure must be reflected in the result

```csharp
try
{
    var searchResult = await _tools.GetTool<IHttpTool>()
        .ExecuteAsync(request, ct);

    if (!searchResult.IsSuccess)
        return Result<AgentResult>.Failure(searchResult.Error!);

    // ... process result
}
catch (OperationCanceledException)
{
    throw; // Always re-throw cancellation
}
catch (Exception ex)
{
    _logger.LogError(ex, "ResearchAgent failed for task {TaskId}", context.TaskId);
    return Result<AgentResult>.Failure(Error.Unexpected(ex.Message));
}
```

---

## Memory Usage

Agents may read from and write to memory via `context.Memory`:

- Read prior research or intermediate outputs to avoid redundant work
- Write intermediate results so later agents in the pipeline can consume them
- Never store memory in instance fields — agents should be stateless between calls
- Memory keys should be namespaced: `"{ProjectId}:{AgentName}:{Key}"`

---

## Tool Access

Agents access tools via `IToolRegistry`:

```csharp
var httpTool = context.Tools.Get<IHttpTool>();
var result = await httpTool.ExecuteAsync(new HttpToolRequest { Url = url }, ct);
```

- Always check tool availability — a tool may not be installed
- Never instantiate tools directly — always resolve through the registry
- If a required tool is not available, return a `Result.Failure` with a descriptive error

---

## Agent Design Rules

1. One agent, one responsibility
2. No direct I/O — always through Tools
3. Always report progress
4. Always support cancellation
5. Return `Result<T>` — never throw for expected failures
6. Never store state in instance fields between executions
7. Always be independently unit-testable with mocked tools
8. Keep constructors small — inject only what is needed

---

## Creating a New Agent

When a new agent is introduced:

1. Create a new project: `FutureBox.Agents.{Name}/`
2. Define the agent class implementing `IAgent`
3. Create a corresponding test project: `FutureBox.Agents.{Name}.Tests/`
4. Register the agent in DI via an extension method
5. Update `agent-guidelines.md` with the new agent's responsibility
6. Update `roadmap.md` if the agent belongs to a new milestone
7. Record the decision in `decisions.md` if the agent introduces a new pattern
