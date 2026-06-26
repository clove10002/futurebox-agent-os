# FutureBox Agent OS — Changelog

This document records meaningful changes to the FutureBox platform in chronological order.  
Update this file after completing significant milestones or architectural changes.

---

## [Unreleased] — MVP In Progress

### Added

- `/docs` directory established as the project's documentation source of truth
- `vision.md` — project mission, philosophy, goals, non-goals, and future capabilities
- `architecture.md` — system layers, dependency rules, agent model, tool model, orchestrator, memory, SignalR communication
- `coding-standards.md` — C# naming conventions, DI rules, async patterns, Result pattern, logging, testing strategy
- `agent-guidelines.md` — agent responsibilities, lifecycle, IAgent interface, progress reporting, error handling, memory usage
- `tool-guidelines.md` — tool design rules, ITool interface, tool catalog, registry pattern
- `roadmap.md` — MVP scope and deliverables, version milestones through 1.0, future ideas
- `decisions.md` — Architecture Decision Records: Clean Architecture, .NET 9 + Blazor, SQLite, SignalR, Agent/Tool separation, Documentation-Driven Development
- `project-structure.md` — full repository and project layout, naming conventions, structural rules
- `development-workflow.md` — 8-step development process, architecture review checklist
- `changelog.md` — this file

### Architecture Decisions Recorded

- ADR-001: Clean Architecture with strict dependency rules
- ADR-002: .NET 9 + Blazor Web App
- ADR-003: SQLite for MVP, PostgreSQL later
- ADR-004: SignalR as real-time communication layer
- ADR-005: Separate Agent and Tool concepts
- ADR-006: Documentation-Driven Development as project workflow
