# FutureBox Agent OS — Coding Standards

All code generated or contributed to FutureBox must comply with these standards.  
Standards exist to make the codebase readable, maintainable, and consistent across time and contributors.

---

## Naming Conventions

| Element | Convention | Example |
|---|---|---|
| Class | PascalCase | `ResearchAgent`, `ProjectService` |
| Interface | PascalCase with `I` prefix | `IAgent`, `IToolRegistry` |
| Method | PascalCase | `ExecuteAsync`, `CreateProject` |
| Property | PascalCase | `ProjectId`, `AgentName` |
| Private field | `_camelCase` | `_logger`, `_repository` |
| Parameter | camelCase | `cancellationToken`, `projectId` |
| Local variable | camelCase | `executionPlan`, `result` |
| Constant | PascalCase | `DefaultTimeout`, `MaxRetries` |
| Enum | PascalCase (type and values) | `AgentState.Running` |
| Generic type parameter | `T`, or descriptive `TResult` | `Result<TValue>` |
| Async method | Suffix with `Async` | `ExecuteAsync`, `GetProjectAsync` |
| Test class | `{Subject}Tests` | `ResearchAgentTests` |
| Test method | `{Method}_{Scenario}_{ExpectedResult}` | `Execute_WhenTopicIsEmpty_ReturnsFailure` |

---

## File and Folder Organization

- One public type per file
- File name matches the type name exactly: `ResearchAgent.cs`, `IAgent.cs`
- Folder structure mirrors namespace structure
- No `Helpers`, `Utils`, or `Misc` folders — name by responsibility
- Tests mirror the source structure under a `/tests` root project

---

## Dependency Injection

- All services are registered via DI — never use `new` for services
- Never use the Service Locator pattern — inject dependencies, do not resolve them
- Constructor injection is the default; property injection only for optional, rarely used dependencies
- Register interfaces, not concrete types, whenever the implementation may change
- Extension methods on `IServiceCollection` for module registration:

```csharp
public static class ResearchAgentServiceCollectionExtensions
{
    public static IServiceCollection AddResearchAgent(this IServiceCollection services)
    {
        services.AddScoped<IAgent, ResearchAgent>();
        return services;
    }
}
```

---

## Async Programming

- All I/O-bound operations must be `async`/`await`
- Never use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` — this causes deadlocks
- Every public async method accepts `CancellationToken ct` as the **last parameter**
- Pass `CancellationToken` through the entire call chain — never ignore it
- Use `ConfigureAwait(false)` in library/infrastructure code; omit it in Blazor UI code
- Avoid `async void` — only acceptable for Blazor event handlers where necessary

```csharp
// Correct
public async Task<Result<AgentResult>> ExecuteAsync(AgentContext context, CancellationToken ct)
{
    var data = await _repository.GetAsync(context.ProjectId, ct).ConfigureAwait(false);
    ...
}
```

---

## Result Pattern

Operations that can fail should return `Result<T>` rather than throwing exceptions.  
Exceptions are reserved for truly exceptional, unexpected states.

```csharp
// Domain result type
public readonly record struct Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error? Error { get; }

    public static Result<T> Success(T value) => ...;
    public static Result<T> Failure(Error error) => ...;
}
```

- Never use exceptions for control flow (e.g., "not found" is not an exception)
- Catch exceptions at infrastructure boundaries and convert them to `Result.Failure`
- Let unhandled exceptions propagate only when they represent system-level failures

---

## Logging

- Use `ILogger<T>` — never `Console.WriteLine`, `Debug.WriteLine`, or static loggers
- Use structured logging with named parameters:

```csharp
_logger.LogInformation("Agent {AgentName} started task {TaskId}", Name, context.TaskId);
```

- Use appropriate levels: `LogDebug` for internals, `LogInformation` for significant events, `LogWarning` for recoverable issues, `LogError` for failures
- Never log sensitive data (API keys, personal information)

---

## Exception Handling

- Catch only what you can handle — do not catch `Exception` broadly unless at a boundary
- Always log before swallowing an exception
- Use `OperationCanceledException` handling for cancellation:

```csharp
catch (OperationCanceledException)
{
    _logger.LogInformation("Task {TaskId} was cancelled", taskId);
    throw; // Re-throw — cancellation is not an error
}
```

---

## Class Design

- Classes have one responsibility (Single Responsibility Principle)
- Maximum class size guideline: if a class exceeds ~300 lines, consider splitting it
- Prefer composition over inheritance
- Avoid `static` methods and fields for anything stateful
- Avoid `partial` classes except for Blazor code-behind patterns
- Use `sealed` on classes not intended for inheritance
- Use `record` for immutable data structures (entities, value objects, DTOs)

---

## SOLID Principles

| Principle | Application |
|---|---|
| Single Responsibility | Each class/service has one reason to change |
| Open/Closed | Extend via new agents/tools, not modifying existing ones |
| Liskov Substitution | Agent/Tool implementations are fully substitutable |
| Interface Segregation | Prefer small, focused interfaces over large general ones |
| Dependency Inversion | Depend on `IAgent`, `ITool`, `IRepository` — never on concrete classes |

---

## Testing Strategy

- Unit tests: test individual classes in isolation with mocked dependencies
- Integration tests: test full request/response cycles against a real SQLite instance
- No mocking of the database in integration tests — use a real SQLite in-memory database
- Every public method on a service or agent must have corresponding tests
- Tests must not depend on execution order
- Aim for meaningful coverage of business logic, not line coverage targets

Test projects:
```
FutureBox.Domain.Tests
FutureBox.Application.Tests
FutureBox.Infrastructure.Tests
FutureBox.Agents.Research.Tests
```

---

## Configuration

- Use the **Options Pattern** (`IOptions<T>`, `IOptionsSnapshot<T>`) for all configuration
- Never read `IConfiguration` directly inside a service — bind to a typed options class
- Validate options at startup using `ValidateDataAnnotations()` or `ValidateOnStart()`
- Never hardcode connection strings, API keys, or endpoints

```csharp
public class LlmOptions
{
    public required string ApiKey { get; init; }
    public required string Model { get; init; }
    public int MaxTokens { get; init; } = 4096;
}
```

---

## Code Formatting

- Indent with 4 spaces (no tabs)
- Use `var` when the type is evident from the right-hand side
- Use expression-bodied members only for simple, single-expression properties/methods
- Prefer explicit access modifiers (`public`, `private`, `internal`) on all members
- Order class members: fields → constructors → properties → public methods → private methods
- Use `using` declarations (C# 8+) over `using` statements where appropriate

---

## Comments

- Write code that explains itself through naming
- Add a comment only when the **why** is non-obvious: a hidden constraint, a workaround, a subtle invariant
- Never write comments that describe **what** the code does — the code already says that
- XML doc comments (`///`) are required on all public interfaces and their members
- Do not write task references or author names in comments
