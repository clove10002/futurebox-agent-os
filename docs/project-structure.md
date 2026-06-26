# FutureBox Agent OS — Project Structure

Every file lives in a defined location. Deviations require a discussion and a documentation update.

---

## Repository Root

```
futurebox-agent-os/
│
├── docs/                          ← Documentation (source of truth)
│   ├── vision.md
│   ├── architecture.md
│   ├── coding-standards.md
│   ├── agent-guidelines.md
│   ├── tool-guidelines.md
│   ├── roadmap.md
│   ├── decisions.md
│   ├── project-structure.md
│   ├── development-workflow.md
│   └── changelog.md
│
├── src/                           ← Core platform source
│   ├── FutureBox.Domain/
│   ├── FutureBox.Application/
│   ├── FutureBox.Infrastructure/
│   ├── FutureBox.Presentation/
│   └── FutureBox.Shared/
│
├── agents/                        ← One project per agent
│   ├── FutureBox.Agents.Research/
│   ├── FutureBox.Agents.Script/
│   ├── FutureBox.Agents.Narration/
│   ├── FutureBox.Agents.Subtitle/
│   ├── FutureBox.Agents.Asset/
│   └── FutureBox.Agents.Video/
│
├── tools/                         ← One project per tool
│   ├── FutureBox.Tools.Http/
│   ├── FutureBox.Tools.File/
│   ├── FutureBox.Tools.Llm/
│   ├── FutureBox.Tools.Tts/
│   └── FutureBox.Tools.FFmpeg/
│
├── tests/                         ← One test project per source project
│   ├── FutureBox.Domain.Tests/
│   ├── FutureBox.Application.Tests/
│   ├── FutureBox.Infrastructure.Tests/
│   ├── FutureBox.Agents.Research.Tests/
│   ├── FutureBox.Agents.Script.Tests/
│   └── FutureBox.Tools.Http.Tests/
│
├── FutureBox.sln
├── .editorconfig
├── .gitignore
├── Directory.Build.props           ← Shared MSBuild properties
├── Directory.Packages.props        ← Centralized NuGet package versions
└── README.md
```

---

## Layer Internals

### FutureBox.Domain

```
FutureBox.Domain/
├── Entities/
│   ├── Project.cs
│   ├── ExecutionPlan.cs
│   ├── AgentTask.cs
│   └── ProjectOutput.cs
├── Interfaces/
│   ├── IAgent.cs
│   ├── ITool.cs
│   ├── IOrchestrator.cs
│   ├── IToolRegistry.cs
│   ├── IMemoryStore.cs
│   ├── IProgressReporter.cs
│   └── IAgentDispatcher.cs
├── Events/
│   └── AgentTaskCompletedEvent.cs
└── ValueObjects/
    ├── ProjectId.cs
    └── TaskId.cs
```

### FutureBox.Application

```
FutureBox.Application/
├── Services/
│   ├── ProjectService.cs
│   ├── WorkflowService.cs
│   ├── PlannerService.cs
│   └── ExecutionTracker.cs
├── Orchestration/
│   └── Orchestrator.cs
├── Interfaces/
│   ├── IProjectRepository.cs
│   └── INotificationService.cs
└── Extensions/
    └── ServiceCollectionExtensions.cs
```

### FutureBox.Infrastructure

```
FutureBox.Infrastructure/
├── Persistence/
│   ├── FutureBoxDbContext.cs
│   ├── Repositories/
│   │   └── ProjectRepository.cs
│   └── Migrations/
├── Notifications/
│   └── SignalRNotificationService.cs
└── Extensions/
    └── ServiceCollectionExtensions.cs
```

### FutureBox.Presentation (Blazor Web App)

```
FutureBox.Presentation/
├── Components/
│   ├── App.razor
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   └── Pages/
│       ├── Dashboard.razor
│       ├── NewProject.razor
│       └── ProjectDetail.razor
├── Hubs/
│   └── ExecutionHub.cs
├── Program.cs
└── appsettings.json
```

### FutureBox.Shared

```
FutureBox.Shared/
├── Results/
│   ├── Result.cs
│   └── Error.cs
├── Enumerations/
│   ├── ProjectStatus.cs
│   ├── AgentState.cs
│   └── TaskStatus.cs
└── Extensions/
    └── StringExtensions.cs
```

### Agent Project (example: FutureBox.Agents.Research)

```
FutureBox.Agents.Research/
├── ResearchAgent.cs
├── ResearchOptions.cs
├── Models/
│   ├── ResearchResult.cs
│   └── SourceReference.cs
└── Extensions/
    └── ServiceCollectionExtensions.cs
```

### Tool Project (example: FutureBox.Tools.Http)

```
FutureBox.Tools.Http/
├── HttpTool.cs
├── IHttpTool.cs
├── HttpToolRequest.cs
├── HttpToolResult.cs
├── HttpToolOptions.cs
└── Extensions/
    └── ServiceCollectionExtensions.cs
```

---

## Naming Conventions for Projects

| Purpose | Pattern | Example |
|---|---|---|
| Core layer | `FutureBox.{Layer}` | `FutureBox.Domain` |
| Agent | `FutureBox.Agents.{Name}` | `FutureBox.Agents.Research` |
| Tool | `FutureBox.Tools.{Name}` | `FutureBox.Tools.FFmpeg` |
| Tests | `{SourceProject}.Tests` | `FutureBox.Agents.Research.Tests` |
| Future plugin | `FutureBox.Plugins.{Name}` | `FutureBox.Plugins.YouTube` |

---

## Rules

- One class per file; filename matches type name
- Namespace matches folder path exactly
- No circular project references
- No `Helpers`, `Utils`, or `Misc` folders — name by responsibility
- Infrastructure never references Presentation
- Domain never references any other FutureBox project
