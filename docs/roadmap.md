# FutureBox Agent OS — Roadmap

Features ship only when they belong to the current milestone.  
The platform must always be in a working state.  
Reliability takes priority over feature breadth at every milestone.

---

## MVP — Video Creation Workflow

**Goal:** Deliver one complete, reliable, end-to-end workflow.  
**Status:** In Progress

### Scope

Input: A topic provided by the user.

Workflow:

```
Topic
  └─► ResearchAgent   (HttpTool, LlmTool)
        └─► ScriptAgent    (LlmTool)
              └─► NarrationAgent  (TtsTool)
                    └─► SubtitleAgent  (LlmTool or WhisperTool)
                          └─► AssetAgent   (HttpTool, FileTool)
                                └─► VideoAgent   (FfmpegTool)
                                      └─► video.mp4
```

Output: `video.mp4` saved to the project output directory.

### Deliverables

- [ ] Solution scaffolding with all layer projects
- [ ] Core domain entities: `Project`, `ExecutionPlan`, `AgentTask`, `AgentResult`
- [ ] Core interfaces: `IAgent`, `ITool`, `IOrchestrator`, `IMemoryStore`
- [ ] Orchestrator with PlannerService and AgentDispatcher
- [ ] ResearchAgent + HttpTool
- [ ] ScriptAgent + LlmTool
- [ ] NarrationAgent + TtsTool
- [ ] SubtitleAgent
- [ ] AssetAgent + FileTool
- [ ] VideoAgent + FfmpegTool
- [ ] SQLite persistence for Projects and execution history
- [ ] SignalR hub for real-time progress
- [ ] Blazor UI: goal input, live progress view, output display
- [ ] Basic unit and integration tests for each agent
- [ ] Complete end-to-end test: topic → video.mp4

### Success Criteria

- A user types a topic and receives a completed video.mp4
- Progress is visible in the UI throughout execution
- Failures report clearly without crashing the application
- The workflow can be retried from where it failed

---

## Version 0.2 — Platform Foundations

**Goal:** Transform the single workflow into a reusable platform.

### Scope

- Multiple workflow types (not just video)
- Project history: list, view, reopen past projects
- Basic task-level memory (avoid redundant research across steps)
- Improved UI: operations center layout, agent status panel, log viewer
- Error recovery: retry failed tasks from checkpoint
- Configuration UI: API keys, model selection, output paths
- Plugin loading groundwork: agents/tools discoverable from config

---

## Version 0.5 — Agent Ecosystem

**Goal:** Expand the agent ecosystem and enable Windows automation.

### Scope

- CodingAgent: generate, test, and run code
- BrowserAgent + ChromeTool (Playwright): browser automation
- WindowsAgent + MouseTool + KeyboardTool + ScreenCaptureTool: desktop automation
- OcrTool: extract text from screen regions
- Parallel agent execution: multiple agents running concurrently within a workflow
- Memory: persistent project and knowledge memory across sessions
- Improved planner: dynamic plan adjustment based on execution results
- Agent-to-agent communication (sub-tasks)

---

## Version 1.0 — Open-Source Platform

**Goal:** First stable public release as a professional open-source platform.

### Scope

- Plugin marketplace architecture: install agents/tools as packages
- Plugin manifest: capability declarations, version, dependencies
- Voice input: speak a goal; FutureBox transcribes and executes
- Scheduled workflows: time-triggered and event-triggered
- PostgreSQL support (configurable switch from SQLite)
- Multi-user support (per-user projects and history)
- Full documentation site
- Contributor guide and architecture guide
- CI/CD pipeline
- Comprehensive test coverage (unit, integration, E2E)

---

## Future Ideas (Post 1.0)

These are directional signals, not commitments:

- Distributed execution: agents running across multiple machines
- Agent composition: agents delegating sub-tasks to other agents
- Vector database for semantic long-term memory
- Document Agent: PDFs, reports, spreadsheets
- Data Analysis Agent: charts, insights from datasets
- Scheduling Agent: calendar-aware workflows
- External platform integrations (YouTube, Notion, Slack, Google Drive)
- Real-time collaboration: multiple users observing a shared project
- Mobile observer app for monitoring workflows
