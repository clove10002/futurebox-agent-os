# FutureBox Agent OS — Tool Guidelines

Tools are the capability layer of FutureBox. They perform concrete actions against external software, APIs, and the operating system. They contain no business logic. They are interchangeable.

---

## Core Principle

**Tools perform actions. Agents solve problems.**

A tool does exactly one thing. It does not decide when, why, or whether to do it.  
That decision belongs to the agent.

This separation ensures:
- Tools can be replaced without modifying agents
- Tools can be tested in isolation
- The platform remains extensible and composable

---

## ITool Interface

Every tool implements `ITool`:

```csharp
/// <summary>
/// Represents a capability that can perform a single type of action.
/// </summary>
public interface ITool
{
    /// <summary>
    /// Unique name identifying this tool type.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Human-readable description of what this tool does.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executes the tool's action with the given request.
    /// </summary>
    Task<ToolResult> ExecuteAsync(ToolRequest request, CancellationToken ct);
}
```

For type safety, tools should expose a typed generic interface in addition to `ITool`:

```csharp
public interface IHttpTool : ITool
{
    Task<ToolResult<HttpResponse>> ExecuteAsync(HttpToolRequest request, CancellationToken ct);
}
```

---

## ToolRequest and ToolResult

Tools communicate via typed request/result objects:

```csharp
public record HttpToolRequest : ToolRequest
{
    public required string Url { get; init; }
    public HttpMethod Method { get; init; } = HttpMethod.Get;
    public IReadOnlyDictionary<string, string>? Headers { get; init; }
    public string? Body { get; init; }
}

public record HttpToolResult : ToolResult
{
    public required int StatusCode { get; init; }
    public required string Content { get; init; }
    public bool IsSuccess => StatusCode is >= 200 and < 300;
}
```

Never use `object`, `dynamic`, or untyped dictionaries for request/result data.

---

## Tool Catalog

### Current Tools (MVP)

| Tool | Project | Capability |
|---|---|---|
| `HttpTool` | `FutureBox.Tools.Http` | HTTP GET/POST requests |
| `FileTool` | `FutureBox.Tools.File` | Read/write files to the project output directory |
| `LlmTool` | `FutureBox.Tools.Llm` | Send prompts to an LLM and receive text responses |
| `FfmpegTool` | `FutureBox.Tools.FFmpeg` | Compose video and audio using FFmpeg |
| `TtsTool` | `FutureBox.Tools.Tts` | Convert text to narration audio |

### Planned Tools (future milestones)

| Tool | Capability |
|---|---|
| `ChromeTool` | Browser automation (Playwright or Selenium) |
| `WhisperTool` | Speech-to-text transcription |
| `PowerShellTool` | Execute PowerShell scripts |
| `ScreenCaptureTool` | Capture screenshots |
| `MouseTool` | Simulate mouse input |
| `KeyboardTool` | Simulate keyboard input |
| `OcrTool` | Read text from images |
| `PhotoshopTool` | Automate Adobe Photoshop |
| `DaVinciTool` | Automate DaVinci Resolve |

---

## Design Rules

### 1. Tools are stateless
Tools must not store execution state in instance fields between calls.  
All state relevant to an execution lives in the `ToolRequest`.

### 2. Tools never contain business logic
A tool does not decide what URL to fetch, what prompt to send, or what filename to use.  
That is the agent's job. The tool executes the instruction.

### 3. Tools are replaceable
If `FfmpegTool` can be replaced with a cloud video rendering tool, swapping the implementation should require zero changes to any agent.

### 4. Tools never call other tools
A tool does not depend on another tool.  
If a composite action is needed, the agent coordinates multiple tool calls.

### 5. Tools always support cancellation
Every tool must pass `CancellationToken` through to every I/O operation.

### 6. Tools handle their own infrastructure errors
Network timeouts, file permission errors, process crashes — the tool catches these and returns a `ToolResult` with `IsSuccess = false` and a descriptive error.  
Tools do not throw for expected failures.

---

## Error Handling

```csharp
public async Task<ToolResult<HttpResponse>> ExecuteAsync(HttpToolRequest request, CancellationToken ct)
{
    try
    {
        var response = await _httpClient.GetAsync(request.Url, ct).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        return ToolResult<HttpResponse>.Success(new HttpResponse
        {
            StatusCode = (int)response.StatusCode,
            Content = content
        });
    }
    catch (OperationCanceledException)
    {
        throw; // Always re-throw cancellation
    }
    catch (HttpRequestException ex)
    {
        _logger.LogWarning(ex, "HTTP request failed for URL {Url}", request.Url);
        return ToolResult<HttpResponse>.Failure($"HTTP request failed: {ex.Message}");
    }
}
```

---

## Tool Registry

Tools are resolved by agents through `IToolRegistry`:

```csharp
public interface IToolRegistry
{
    Tool Get<TInterface>() where TInterface : ITool;
    bool IsAvailable<TInterface>() where TInterface : ITool;
    IReadOnlyList<ITool> GetAll();
}
```

Agents should check `IsAvailable` before calling a tool that may not be installed.

---

## Creating a New Tool

When a new tool is introduced:

1. Create a new project: `FutureBox.Tools.{Name}/`
2. Define the typed interface extending `ITool`
3. Implement the tool class
4. Define `ToolRequest` and `ToolResult` record types
5. Register the tool in DI via an extension method
6. Create a corresponding test project: `FutureBox.Tools.{Name}.Tests/`
7. Update `tool-guidelines.md` with the new tool in the catalog
8. Record the decision in `decisions.md` if the tool introduces a new integration or pattern
