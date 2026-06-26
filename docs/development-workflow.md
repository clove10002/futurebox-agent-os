# FutureBox Agent OS — Development Workflow

This workflow applies to every feature, agent, tool, or architectural change introduced to FutureBox.  
Do not skip steps. Documentation is not optional.

---

## Standard Feature Workflow

```
1. Understand
2. Review Documentation
3. Identify Components
4. Propose Architecture (if needed)
5. Implement Incrementally
6. Test
7. Update Documentation
8. Record Decisions
```

---

### Step 1 — Understand

Before writing anything, fully understand:

- What is the user's goal?
- What should the system do that it cannot do today?
- What is the expected input and output?
- What are the edge cases and failure modes?

Ask clarifying questions before proposing an implementation if the goal is ambiguous.

---

### Step 2 — Review Documentation

Consult the relevant documentation before designing the solution:

| Change type | Review |
|---|---|
| New feature | `vision.md`, `roadmap.md` |
| New agent | `agent-guidelines.md`, `architecture.md` |
| New tool | `tool-guidelines.md`, `architecture.md` |
| Architectural change | `architecture.md`, `decisions.md` |
| Code change | `coding-standards.md` |
| New milestone | `roadmap.md` |

If the documentation conflicts with the requested change, flag it before proceeding.  
If documentation is missing for the change being made, create it first.

---

### Step 3 — Identify Affected Components

Map the change to the architecture:

- Which layer owns this? (Domain, Application, Infrastructure, Presentation, Shared)
- Which agent(s) are involved?
- Which tool(s) are involved?
- What interfaces need to be created or extended?
- What data entities are affected?
- What tests need to be written or updated?

---

### Step 4 — Propose Architecture (if new patterns are introduced)

If the implementation introduces:

- A new layer interaction not previously documented
- A new pattern (e.g., first use of CQRS, a new memory type, a new plugin mechanism)
- A deviation from documented architecture

Then **propose the design first**. Get alignment before implementing.  
Document the decision in `decisions.md` before or immediately after implementing.

Do not implement first and document later for significant architectural decisions.

---

### Step 5 — Implement Incrementally

- Implement one component at a time
- Commit early, commit often — small, focused commits
- Keep interfaces and domain types stable; let infrastructure evolve
- Do not add features that belong to a later milestone
- Do not over-engineer for hypothetical future requirements
- If a shortcut is taken for speed, flag it explicitly and create a follow-up task

---

### Step 6 — Test

Write tests alongside implementation, not after:

- Unit tests for every service, agent, and tool method
- Integration tests for repository implementations and end-to-end flows
- Tests must be deterministic and order-independent
- All tests must pass before considering a change complete

---

### Step 7 — Update Documentation

After implementation, update the relevant documentation:

| What changed | Update |
|---|---|
| New agent introduced | `agent-guidelines.md`, `project-structure.md` |
| New tool introduced | `tool-guidelines.md`, `project-structure.md` |
| Architecture changed | `architecture.md` |
| New project structure | `project-structure.md` |
| Milestone completed | `roadmap.md`, `changelog.md` |
| New coding rule | `coding-standards.md` |

Documentation is a first-class artifact. Leaving it out of sync is a defect.

---

### Step 8 — Record Decisions

If the implementation involved a significant design decision:

- Add an ADR entry to `decisions.md`
- Include: context, problem, options considered, decision, consequences
- Date every entry

Decisions recorded now prevent architectural drift six months from now.

---

## Architecture Review Checklist

Before finalizing any change, verify:

- [ ] The dependency rule is not violated (Domain → nothing, Application → Domain only)
- [ ] No new static state or service locator usage was introduced
- [ ] All new services are registered via DI
- [ ] All new async methods accept `CancellationToken`
- [ ] Error cases return `Result<T>` — not raw exceptions for expected failures
- [ ] Progress is reported via `IProgressReporter` — not console or direct logging
- [ ] New agents and tools are independently testable
- [ ] Documentation has been updated

---

## What NOT to do

- Do not implement features that belong to a future milestone without explicit instruction
- Do not silently introduce architectural patterns that contradict `architecture.md`
- Do not skip tests to ship faster — reliability is the MVP's primary goal
- Do not write documentation after the fact as a low-priority afterthought
- Do not resolve architectural ambiguity by guessing — ask first
