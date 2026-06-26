# FutureBox Agent OS — Architecture

## Overview

FutureBox follows **Clean Architecture** with strict, unidirectional dependency rules.  
The system is composed of layers, each with a single, well-defined responsibility.  
No layer may depend on a layer outside of it. All major contracts are interfaces.

---

## Architectural Layers

```
┌──────────────────────────────────────────────┐
│              React + Vite + Tailwind         │  Frontend (separate dev server, served by API in prod)
├──────────────────────────────────────────────┤
│         ASP.NET Core Minimal APIs            │  HTTP endpoints, Swagger, SignalR Hubs
├──────────────────────────────────────────────┤
│                 Application                  │  Use cases, Orchestration, Services
├──────────────────────────────────────────────┤
│                   Domain                     │  Entities, Interfaces, Domain Rules
├──────────────────────────────────────────────┤
│                Infrastructure                │  SQL Server, External APIs, Tool Impls
├──────────────────────────────────────────────┤
│                   Shared                     │  DTOs, Enums, Results, Constants
└──────────────────────────────────────────────┘
         Agents          Tools          Plugins
```

### FutureBox.Domain
The core of the system. Has **zero external dependencies**.

Contains:
- Core entities (`Project`, `ExecutionPlan`, `AgentTask`, `ToolResult`, etc.)
- Domain interfaces (`IAgent`, `ITool`, `IOrchestrator`, `IMemoryStore`, etc.)
- Domain events
- Value objects and enumerations
- Domain rules and invariants

### FutureBox.Application
Contains business logic and use case orchestration. Depends only on `Domain`.

Contains:
- Application services (`ProjectService`, `WorkflowService`, etc.)
- Orchestrator implementation coordination
- Command/query handlers (if CQRS is introduced later)
- Application-level interfaces (e.g., `IProjectRepository`, `INotificationService`)
- DTO mappings

### FutureBox.Infrastructure
Implements interfaces defined in `Application` and `Domain`. Never referenced by `Domain` or `Application` directly — only through DI.

Contains:
- Database access (SQL Server via Entity Framework Core)
- Repository implementations
- External API clients (AI providers, YouTube API, etc.)
- Concrete tool implementations (or tool host — tools live in `FutureBox.Tools.*`)
- Hosted services and background workers

### FutureBox.Api
The ASP.NET Core Minimal APIs host. Depends on `Application` only — never on `Infrastructure` or `Domain` directly beyond shared contracts.

Contains:
- Minimal API endpoint definitions
- SignalR hub registrations
- Swagger / OpenAPI configuration
- Dependency injection wiring (composition root)
- Static file serving (React build in production)

### futurebox-ui (React)
The React frontend. Communicates with `FutureBox.Api` via HTTP and SignalR.

Contains:
- React + Vite + Tailwind
- Pages: New Project, Execution View, Output View
- SignalR client for real-time agent progress
- Font Awesome Free icons

### FutureBox.Shared
Cross-cutting types with no business logic. May be referenced by any layer.

Contains:
- Result types (`Result<T>`, `Error`)
- Common enumerations (`TaskStatus`, `AgentState`, etc.)
- Extension methods
- Shared constants

---

## Dependency Rules

```
Presentation   → Application, Shared
Application    → Domain, Shared
Infrastructure → Application, Domain, Shared
Domain         → Shared only
Agents.*       → Application, Domain, Shared
Tools.*        → Domain, Shared
```

**The Dependency Rule is absolute:**
- `Domain` never imports from `Application`, `Infrastructure`, or `Presentation`
- `Application` never imports from `Infrastructure` or `Presentation`
- Violations of this rule require an Architecture Decision Record (ADR)

---

## Agent Model

Agents are **goal-solving components**. Each agent has exactly one responsibility.

```
IAgent
├── string Name { get; }
├── string Description { get; }
├── Task<AgentResult> ExecuteAsync(AgentContext context, CancellationToken ct)
└── bool CanHandle(AgentTask task)
```

Rules:
- Agents **never** directly manipulate external software or systems
- Agents **request capabilities** from Tools via `IToolRegistry`
- Agents report progress via `IProgressReporter` — never via console or direct logging
- Agents must support cancellation via `CancellationToken`
- Each agent lives in its own project (`FutureBox.Agents.Research`, etc.)
- Agents are registered in DI and discovered at runtime

---

## Tool Model

Tools are **action-performing components**. They expose one capability each.

```
ITool
├── string Name { get; }
├── string Description { get; }
└── Task<ToolResult> ExecuteAsync(ToolRequest request, CancellationToken ct)
```

Rules:
- Tools **never** contain business logic or decisions
- Tools are **stateless where possible**
- Tools accept a typed `ToolRequest` and return a typed `ToolResult`
- Tools are replaceable — the agent references `ITool`, not the concrete class
- Each tool lives in its own project (`FutureBox.Tools.Http`, `FutureBox.Tools.FFmpeg`, etc.)

---

## Orchestrator

The Orchestrator is the brain of FutureBox. It receives a goal and drives execution to completion.

**Responsibilities:**
- Accept a user goal and project context
- Delegate planning to `IPlannerService`
- Dispatch agent tasks via `IAgentDispatcher`
- Track execution state via `IExecutionTracker`
- Handle and recover from failures via `IRecoveryService`
- Collect outputs and finalize the project
- Emit execution events for the UI

**The Orchestrator must never become a God Class.**  
When complexity grows, extract responsibilities into the dedicated services listed above.

---

## Memory Subsystem

FutureBox supports multiple memory types, each with a replaceable implementation:

| Type | Purpose |
|---|---|
| Conversation Memory | Retains the context of the current interaction |
| Task Memory | Tracks intermediate results within a workflow |
| Project Memory | Stores outputs and history for a specific project |
| Knowledge Memory | Long-term factual storage (future: vector DB) |
| Execution History | Logs of past runs, agent activity, timing |

All memory implementations are hidden behind `IMemoryStore<T>`.

---

## Projects Subsystem

Every user goal creates a **Project**. Projects are the unit of work and the unit of history.

A `Project` contains:
- Unique identifier and name
- Goal text
- Current status
- Associated `ExecutionPlan`
- All generated files and outputs
- Execution logs
- Configuration snapshot
- Timestamps

FutureBox never loses track of generated work.

---

## Communication & Real-Time Updates

SignalR is the real-time communication layer between the ASP.NET Core API and the React frontend.

Events emitted to the UI:
- Agent started / completed / failed
- Task progress updates
- Log entries
- File outputs produced
- Workflow phase transitions
- Final completion or failure reports

Every significant execution event must flow through SignalR. The UI is an observer of the execution engine, not a controller.

---

## Plugin Architecture

The plugin system is a long-term goal, but every design decision must be made with it in mind.

- Agents and Tools are already interface-first by design
- New agents/tools can be added without modifying core code (Open/Closed Principle)
- Future: agents/tools loaded from external assemblies via `IPluginLoader`
- Future: plugin manifest files describing capabilities, dependencies, and configuration

---

## Data Flow (MVP)

```
User provides Topic
        │
        ▼
ProjectService.CreateProjectAsync()
        │
        ▼
Orchestrator.ExecuteAsync(goal)
        │
        ▼
PlannerService → ExecutionPlan (ordered AgentTasks)
        │
        ▼
AgentDispatcher → ResearchAgent → HttpTool
        │
        ▼
AgentDispatcher → ScriptAgent → LLM API
        │
        ▼
AgentDispatcher → NarrationAgent → TTS API
        │
        ▼
AgentDispatcher → VideoAgent → FFmpegTool
        │
        ▼
ExecutionTracker → Project outputs saved
        │
        ▼
UI updated via SignalR throughout
```
