# Implementation Plan: MVP Subscription Management

**Branch**: `001-subscription-management` | **Date**: 2026-03-08 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/001-subscription-management/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

**Core MVP Feature**: Enable users to add and view RSS/Atom feed subscriptions through a web UI. The MVP focuses exclusively on subscription management (add URL, display list) with in-memory storage. No feed fetching, parsing, or persistence required for this phase.

**Technical Approach**: ASP.NET Core Web API backend exposes two endpoints (POST to add, GET to list subscriptions). Blazor WebAssembly frontend provides an input form and subscription list display. State management via in-memory collection (List<string>) on backend, reactive binding in Blazor UI.

## Technical Context

**Language/Version**: C# 12 / .NET 8 (LTS)  
**Primary Dependencies**: ASP.NET Core 8.0 Web API, Blazor WebAssembly 8.0, .NET CLI tooling  
**Storage**: In-memory List<string> for MVP (no database required)  
**Testing**: xUnit for unit tests, Blazor UI testing via component tests  
**Target Platform**: Windows, macOS, Linux (cross-platform via .NET)  
**Project Type**: Full-stack web application (backend API + frontend SPA)  
**Performance Goals**: API response <500ms for all operations, UI updates <100ms, handle 10+ subscriptions without degradation  
**Constraints**: Single-user session, in-memory storage only (no persistence), no network operations (no feed fetching), no URL validation  
**Scale/Scope**: Single user, proof-of-concept demonstration, minimal UI polish

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Gate Results**: 1 JUSTIFIED VIOLATION, 4 PASSES

| Principle | Status | Finding |
|-----------|--------|---------|
| I. MVP-First Delivery | ✅ PASS | Feature is pure MVP: add subscriptions, display list, zero extra responsibility. Deferred features (persistence, fetching) clearly documented. |
| II. Security by Default | ⚠️ JUSTIFIED VIOLATION | Spec explicitly requires "System MUST accept any URL without validation", creating tension with Constitution's "All incoming data MUST be validated". **Justification**: MVP is proof-of-concept with single trusted user; validation deferred to Extended-MVP per stakeholder guidance. **Mitigation**: Frontend will trim/validate length; backend will log suspicious patterns. Will add validation when user authentication added. |
| III. Code Quality & Maintainability | ✅ PASS | Public APIs will have XML docs, unit tests required for business logic (70%+ coverage), code review mandatory, naming conventions enforced (PascalCase types, camelCase locals). |
| IV. Separation of Concerns | ✅ PASS | Backend and frontend are separate projects. Communication via JSON contract (AddSubscriptionRequest/SubscriptionsResponse). Each layer has clear responsibilities. Independent testing enabled. |
| V. Incremental Architecture | ✅ PASS | In-memory storage doesn't couple backend to specific persistence mechanism. API contract decoupled from storage layer. Future switch to database won't require API changes. |

**Decision**: Proceed to Phase 0 with JUSTIFIED VIOLATION documented. Re-evaluate after Phase 1 design.

## Project Structure

### Documentation (this feature)

```text
specs/001-subscription-management/
├── plan.md              # This file (planning output)
├── research.md          # Phase 0 output (NEEDED: resolve validation approach conflict)
├── data-model.md        # Phase 1 output (Subscription entity model)
├── quickstart.md        # Phase 1 output (Quick start guide)
├── contracts/           # Phase 1 output (API contract definitions)
│   └── api-contract.md
└── tasks.md             # Phase 2 output (/speckit.tasks command)
```

### Source Code (repository root)

```text
backend/
├── src/
│   ├── RSSFeedReader.API/
│   │   ├── Controllers/
│   │   │   └── SubscriptionsController.cs
│   │   ├── Models/
│   │   │   ├── AddSubscriptionRequest.cs
│   │   │   ├── SubscriptionsResponse.cs
│   │   │   └── Subscription.cs
│   │   ├── Services/
│   │   │   └── SubscriptionService.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   └── RSSFeedReader.API.Tests/
│       ├── Controllers/
│       │   └── SubscriptionsControllerTests.cs
│       └── Services/
│           └── SubscriptionServiceTests.cs
├── .gitignore
└── RSSFeedReader.sln

frontend/
├── src/
│   ├── RSSFeedReader.UI/
│   │   ├── Pages/
│   │   │   └── Subscriptions.razor      # Main MVP page
│   │   ├── Components/
│   │   │   ├── AddSubscriptionForm.razor
│   │   │   └── SubscriptionList.razor
│   │   ├── Services/
│   │   │   └── SubscriptionApiClient.cs
│   │   ├── Models/
│   │   │   ├── AddSubscriptionRequest.cs
│   │   │   └── SubscriptionDto.cs
│   │   ├── wwwroot/
│   │   │   └── appsettings.json        # Frontend config with backend URL
│   │   ├── App.razor
│   │   ├── Program.cs
│   │   └── RSSFeedReader.UI.csproj
│   └── RSSFeedReader.UI.Tests/
│       └── Components/
│           └── SubscriptionsComponentTests.cs
├── .gitignore
└── Directory.Build.props

docs/
├── local-dev-checklist.md
└── architecture.md
```

**Structure Decision**: Web application with frontend + backend separation. Backend is ASP.NET Core API with in-memory service layer. Frontend is Blazor WebAssembly SPA consuming backend API. Allows independent testing and future scaling.

---

## Phase 0: Research & Analysis (COMPLETE ✅)

**Objective**: Resolve technical unknowns and conflicts identified in planning.

**Research Tasks Completed**:
1. ✅ **Validation Approach**: Resolved conflict between spec "no validation" and Constitution "validate inputs"
   - Decision: Light validation (trim, length check) balances security and MVP speed
   - Details in [research.md](research.md#research-task-1-validation-approach-critical)

2. ✅ **Storage Architecture**: Designed abstraction for future database migration
   - Decision: ISubscriptionRepository interface with InMemorySubscriptionRepository impl
   - Details in [research.md](research.md#research-task-2-in-memory-storage-architecture)

3. ✅ **Component Architecture**: Defined Blazor component structure for MVP
   - Decision: Parent-child components with parent state management
   - Details in [research.md](research.md#research-task-3-blazor-component-architecture-for-mvp)

4. ✅ **CORS Configuration**: Established secure development setup
   - Decision: Explicit per-environment CORS, never wildcard
   - Details in [research.md](research.md#research-task-4-cors-configuration-for-mvp)

5. ✅ **Testing Strategy**: Balanced quality with MVP speed
   - Decision: Unit tests (70%+ coverage) + component tests + integration
   - Details in [research.md](research.md#research-task-5-testing-strategy-for-mvp)

6. ✅ **Error Handling**: Aligned specification with Constitution principles
   - Decision: Happy path + basic recovery, no detailed diagnostics
   - Details in [research.md](research.md#research-task-6-error-handling--user-feedback)

**Output**: [research.md](research.md) - All technical unknowns resolved, no "NEEDS CLARIFICATION" markers.

---

## Phase 1: Design & Contracts (COMPLETE ✅)

**Objective**: Create design artifacts and technical specifications for implementation.

### 1. Data Model Design ✅
**Output**: [data-model.md](data-model.md)

**Key Deliverables**:
- **Subscription Entity**: Id (int), Url (string, max 2000), AddedAt (DateTime)
- **API DTOs**: AddSubscriptionRequest, SubscriptionsResponse, SubscriptionDto
- **Repository Interface**: ISubscriptionRepository abstraction for storage layer
- **Implementation**: InMemorySubscriptionRepository (List<Subscription> backend)
- **Validation Rules**: Max 2000 chars, non-null, trimmed; no format validation
- **Constraints**: No duplicates elimination, allow same URL multiple times
- **Migration Path**: Clear path to database persistence without API changes

### 2. API Contract Design ✅
**Output**: [contracts/api-contract.md](contracts/api-contract.md)

**Key Deliverables**:
- **Two Endpoints**:
  - `GET /api/subscriptions` → Returns all subscriptions
  - `POST /api/subscriptions` → Add new subscription
- **Request/Response Format**: JSON with clear structure
- **Error Handling**: Appropriate HTTP status codes (200, 201, 400, 500)
- **CORS Policy**: Explicit frontend origin, no wildcards
- **Example Flow**: Complete request/response walkthrough
- **Future Extensions**: Versioning, auth, rate limiting documented

### 3. Quick Start Guide ✅
**Output**: [quickstart.md](quickstart.md)

**Key Deliverables**:
- **Project Scaffolding**: Commands to create backend, frontend, solution
- **Configuration**: Program.cs setup for CORS, DI, services
- **Running Instructions**: Terminal commands for both backend and frontend
- **Manual Testing**: Steps to verify MVP works
- **Automated Tests**: How to run unit and component tests
- **Verification Checklist**: Pre-launch validation
- **Troubleshooting**: Common issues and solutions
- **Development Workflow**: Daily development loop with auto-reload
- **Next Steps**: How to progress to Extended-MVP

### Constitution Check - Phase 1 Re-evaluation ✅

**Validated Design Against Constitution**:

| Principle | Status | Phase 1 Finding |
|-----------|--------|-----------------|
| I. MVP-First Delivery | ✅ PASS | Design is pure MVP: add subscriptions, display list. No extras. |
| II. Security by Default | ✅ PASS | Validation added (light), CORS configured explicitly, error messages don't leak internals. |
| III. Code Quality & Maintainability | ✅ PASS | Repository pattern enables testing, DI enables swappable implementations, clear separation of concerns. |
| IV. Separation of Concerns | ✅ PASS | Backend API and frontend SPA completely separate, JSON contract between layers, independent deployment. |
| V. Incremental Architecture | ✅ PASS | In-memory storage abstracted behind interface, API doesn't couple to storage mechanism, migration path clear. |

**Justified Violation Status**: The single violation from Phase 0 (validation requirement) is now RESOLVED by Phase 1 design. Light validation implemented satisfies both Constitution and spec intent.

**Decision**: Design PASSES all constitution gates. Ready for Phase 2 task generation.

---

## Phase 2: Task Generation (NEXT STEP)

**Next Command**: `/speckit.tasks` to generate actionable task list

This command will:
1. Read spec.md (user stories, requirements)
2. Read data-model.md (entities)
3. Read contracts/api-contract.md (endpoints)
4. Generate tasks.md with implementation roadmap

**Expected Output**: [tasks.md](tasks.md) with 40-60 tasks organized by user story and phase.

---

## Plan Summary

| Artifact | Status | Purpose |
|----------|--------|---------|
| plan.md | ✅ Complete | This file; planning and design review |
| research.md | ✅ Complete | Phase 0 research resolving unknowns |
| data-model.md | ✅ Complete | Phase 1 entity and storage design |
| contracts/api-contract.md | ✅ Complete | Phase 1 API contract specification |
| quickstart.md | ✅ Complete | Phase 1 developer quick start guide |
| tasks.md | ⏳ Pending | Phase 2 task generation (next step) |

**Planning Complete**. Ready to proceed to Phase 2: Task Generation and Implementation.
