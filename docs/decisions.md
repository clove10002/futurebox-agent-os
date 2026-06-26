# FutureBox Agent OS — Architecture Decision Records

This document is the project's living record of significant architectural decisions.  
Every entry documents context, the problem, options considered, the decision made, and its consequences.

Consult this document before changing any established pattern.  
Add a new entry before introducing a new pattern that will affect the platform long-term.

---

## ADR-007 — Replace Blazor with React + Vite + Tailwind frontend

**Date:** 2026-06-26
**Status:** Accepted

### Context

The original frontend was Blazor Web App, chosen for full-stack C# consistency. The decision to use a React-based frontend was raised early in the project before any code was written.

### Problem

Blazor's primary advantage is full-stack C#. If the frontend is decoupled from the backend anyway (separate dev server, API-driven), Blazor provides no meaningful benefit over a dedicated JavaScript frontend framework.

### Options Considered

1. Keep Blazor Web App
2. React + Vite + Tailwind (API-driven, separate frontend)
3. Vue or Svelte

### Decision

**React + Vite + Tailwind + Font Awesome Free.**

- React is the industry standard for operations center / dashboard UIs
- Vite provides fast development builds and hot module replacement
- Tailwind gives full design control without Bootstrap's opinionated constraints
- Bootstrap was considered but dropped — it conflicts with Tailwind and adds unnecessary overhead
- Font Awesome Free provides sufficient iconography for MVP

### Consequences

- Frontend and backend are cleanly separated (different dev servers in development)
- In production, ASP.NET Core serves the React build as static files — single deployment
- All real-time updates still flow through SignalR (React uses the SignalR JavaScript client)
- `FutureBox.Presentation` project renamed to `FutureBox.Api`; UI lives in `futurebox-ui/`

---

## ADR-008 — Replace SQLite/PostgreSQL with SQL Server

**Date:** 2026-06-26
**Status:** Accepted

### Context

The original plan was SQLite for MVP, PostgreSQL for production. The user has SQL Server Management Studio 18 installed, indicating SQL Server is already available locally.

### Problem

SQLite has limited tooling for inspecting data during development. PostgreSQL requires additional setup (Docker or installation).

### Options Considered

1. SQLite (MVP) → PostgreSQL (production)
2. SQL Server from day one
3. PostgreSQL from day one

### Decision

**SQL Server** — used in both development and production.

- Already installed via SSMS — zero additional setup
- SSMS provides full visual database management (tables, queries, indexes)
- Entity Framework Core supports SQL Server natively and excellently
- Consistent database engine across all environments — no SQLite/PostgreSQL drift

### Consequences

- EF Core migrations target SQL Server provider (`Microsoft.EntityFrameworkCore.SqlServer`)
- Connection string points to local SQL Server instance in development
- SSMS is used for database inspection and management
- Repository interfaces remain database-agnostic — switching providers later requires only infrastructure changes

---

## ADR-009 — Use ASP.NET Core Minimal APIs instead of full MVC controllers

**Date:** 2026-06-26
**Status:** Accepted

### Context

The backend API needs to expose endpoints for the React frontend and generate Swagger documentation automatically.

### Problem

Full MVC controllers add ceremony (controller classes, action methods, routing attributes) that is unnecessary for a focused API.

### Options Considered

1. ASP.NET Core MVC with Controllers
2. ASP.NET Core Minimal APIs
3. FastAPI (Python) — evaluated and rejected; no advantage over .NET for this project

### Decision

**ASP.NET Core Minimal APIs (.NET 9).**

- Minimal boilerplate — endpoints defined inline or in extension methods
- Swagger/OpenAPI generated automatically via `Microsoft.AspNetCore.OpenApi`
- Same performance as controllers; simpler code
- FastAPI (Python) was considered but rejected — all AI SDKs needed have .NET clients; switching to Python would abandon the established C# architecture for no net gain

### Consequences

- Endpoints are organized by feature in extension methods (not controller classes)
- Swagger UI available at `/swagger` in development
- Backend stays entirely in C# — consistent with domain, application, and infrastructure layers

---

## ADR-001 — Adopt Clean Architecture with strict dependency rules

**Date:** 2026-06-26  
**Status:** Accepted

### Context

FutureBox is intended to be a long-running, open-source platform. It will be extended by multiple contributors and grow to support many agents, tools, and integrations.

### Problem

Without defined architectural boundaries, business logic and infrastructure concerns will mix over time, making the system difficult to test, extend, and maintain.

### Options Considered

1. Layered architecture (no strict dependency rules)
2. Clean Architecture (Domain → Application → Infrastructure, strict inward dependency rule)
3. Modular monolith with feature folders
4. Microservices

### Decision

**Clean Architecture** with strict, enforced dependency rules.

- `Domain` has zero external dependencies
- `Application` depends only on `Domain`
- `Infrastructure` implements `Application`/`Domain` interfaces — never referenced directly
- `Presentation` depends on `Application`
- No circular dependencies at any level

### Consequences

- Higher initial setup cost (more projects, more interfaces)
- Business logic is fully testable without infrastructure
- Infrastructure can be replaced without touching business logic
- Architecture enforces good habits as the codebase grows

---

## ADR-002 — Use .NET 9 and Blazor Web App

**Date:** 2026-06-26  
**Status:** Accepted

### Context

FutureBox needs a technology stack that supports real-time updates, is consistent across server and UI, and has strong ecosystem support.

### Problem

Choosing a UI framework that requires a separate language/runtime increases complexity and maintenance burden.

### Options Considered

1. .NET 9 + Blazor Web App (full-stack C#)
2. .NET 9 + React/TypeScript frontend
3. .NET 9 + Electron for desktop-first

### Decision

**.NET 9 + Blazor Web App** (Server-side interactive rendering with SignalR).

- Single language across the entire stack
- Built-in SignalR support for real-time execution updates
- Strong DI, Options, and Logging integration
- Active Microsoft investment and community

### Consequences

- Strong type safety across the full stack
- SignalR connection is central to the UI experience — must be designed for resilience
- Blazor Server requires a persistent connection per user (acceptable for MVP; revisit at scale)

---

## ADR-003 — SQLite for MVP, PostgreSQL for production

**Date:** 2026-06-26  
**Status:** Accepted

### Context

The MVP requires a persistent data store for projects, execution history, and agent outputs.

### Problem

Setting up PostgreSQL adds operational complexity during early development.

### Options Considered

1. SQLite only (simple, zero-setup)
2. PostgreSQL from day one
3. SQLite for MVP, PostgreSQL later with migration path

### Decision

**SQLite for MVP**. Database access is abstracted behind repository interfaces.  
When PostgreSQL is introduced, only the infrastructure implementations change.

### Consequences

- Fast iteration in development — no Docker or database server required
- Repository interfaces must be designed to be database-agnostic from day one
- EF Core (or Dapper) migrations must be structured for provider switching
- Migration from SQLite to PostgreSQL is a planned infrastructure task, not an afterthought

---

## ADR-004 — SignalR as the real-time communication layer

**Date:** 2026-06-26  
**Status:** Accepted

### Context

The FutureBox UI is an operations center. Users must observe agent activity, task progress, and outputs in real time.

### Problem

Polling the server for status updates is inefficient and produces a poor user experience.

### Options Considered

1. HTTP polling
2. Server-Sent Events (SSE)
3. SignalR (WebSockets with fallback)

### Decision

**SignalR** for all real-time communication between the execution engine and the Blazor UI.

- Built into ASP.NET Core with zero additional dependencies
- Works natively with Blazor Server
- Automatic WebSocket fallback to long polling for constrained environments
- Strongly typed hubs improve maintainability

### Consequences

- All significant execution events must emit a SignalR message — this must be designed into services from the start, not added later
- Hub methods and client events must be versioned as the platform grows
- Blazor Server's existing SignalR connection simplifies the architecture (no separate WebSocket client needed)

---

## ADR-005 — Separate Agent and Tool concepts

**Date:** 2026-06-26  
**Status:** Accepted

### Context

AI agent platforms often conflate "reasoning about what to do" with "performing the action". This creates tight coupling and makes components hard to replace.

### Problem

If agents directly call external APIs, write files, or launch processes, they become untestable and tightly coupled to specific implementations.

### Options Considered

1. Agents do everything (simple but tightly coupled)
2. Agents use a generic "action executor" (too abstract)
3. Agents call Tools via a typed interface (clean separation)

### Decision

**Strict Agent/Tool separation.**

- Agents request capabilities through `IToolRegistry`
- Tools are registered independently and are fully replaceable
- Agents are unit-testable with mocked tools
- Tools are integration-testable without agents

### Consequences

- More interfaces and projects to manage
- Complete testability of both agents and tools in isolation
- Any tool can be replaced (e.g., swap FFmpeg for a cloud renderer) without modifying the agent
- Tools can be reused across multiple agents

---

## ADR-006 — Documentation-Driven Development as the project workflow

**Date:** 2026-06-26  
**Status:** Accepted

### Context

FutureBox is a long-term open-source project intended to have multiple contributors over time.

### Problem

Without a single source of truth, architectural decisions drift, new contributors make inconsistent choices, and the codebase diverges from intent.

### Decision

**Documentation-Driven Development (DDD).**

- The `/docs` directory is the source of truth
- Documentation is updated alongside implementation, never after-the-fact as an afterthought
- Architectural violations require an ADR entry before proceeding
- Every new agent, tool, or architectural pattern is documented before or immediately after introduction

### Consequences

- Slightly higher friction per feature (documentation must be maintained)
- Strong consistency across contributors and time
- New contributors can understand the system from documentation alone
- Decisions are never lost to institutional memory
