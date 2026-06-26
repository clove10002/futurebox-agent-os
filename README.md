# FutureBox Agent OS

An AI Agent Platform where the user provides a **goal** and FutureBox determines the **process**.

FutureBox is not a chatbot. It is not a workflow builder. It is an operating system for AI agents.

---

## What It Does

The user describes what they want to accomplish. FutureBox coordinates a team of specialized AI agents to execute the goal end to end.

**Example:**

> "Create a YouTube video explaining the history of Ran Online."

FutureBox will research, script, narrate, subtitle, collect assets, compose the video, and deliver a completed `video.mp4` — with live progress visible throughout.

---

## MVP

The first milestone delivers one complete, reliable workflow:

```
Topic → Research → Script → Narration → Subtitles → Assets → Video → video.mp4
```

---

## Architecture

FutureBox is built on Clean Architecture with strict dependency rules.

- **Domain** — core entities and interfaces, zero external dependencies
- **Application** — use cases, orchestration, business logic
- **Infrastructure** — database, external APIs, tool implementations
- **Presentation** — Blazor Web App, SignalR real-time updates
- **Agents** — specialized workers, one per capability
- **Tools** — capability implementations, stateless and replaceable

See [`/docs`](./docs) for full documentation.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# / .NET 9 |
| Frontend | Blazor Web App |
| Real-time | SignalR |
| Database | SQLite (MVP) → PostgreSQL |
| DI / Config | Microsoft.Extensions |

---

## Documentation

All documentation lives in [`/docs`](./docs):

- [`vision.md`](./docs/vision.md) — mission, goals, product direction
- [`architecture.md`](./docs/architecture.md) — layers, dependency rules, data flow
- [`roadmap.md`](./docs/roadmap.md) — milestones and deliverables
- [`decisions.md`](./docs/decisions.md) — architecture decision records
- [`coding-standards.md`](./docs/coding-standards.md) — engineering standards
- [`agent-guidelines.md`](./docs/agent-guidelines.md) — how agents are designed
- [`tool-guidelines.md`](./docs/tool-guidelines.md) — how tools are implemented

---

## Status

> Pre-alpha. Documentation and architecture foundations in place. Solution scaffolding in progress.

---

## License

MIT
