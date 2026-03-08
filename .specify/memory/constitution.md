# RSS Feed Reader Constitution

<!-- SYNC IMPACT REPORT -->
<!-- Version: 1.0.0 (initial) → 1.0.0 -->
<!-- Principles: 5 core principles defined; Sections: 2 additional (Security, Quality Gates) -->
<!-- Ratified: 2026-03-08 (initial release) -->
<!-- Files updated: constitution.md (created) -->
<!-- No template propagation needed: First constitution version -->

## Core Principles

### I. MVP-First Delivery
The project MUST follow a minimal viable product approach, focusing on proven working features before complexity. The MVP MUST deliver subscription management (add feed URL, display subscriptions list) before any Extended-MVP features (feed fetching, item display). Each phase MUST be fully working and tested before progress to the next phase. Deferred features MUST be clearly documented for Extended-MVP or post-MVP phases.

**Rationale**: Rapid delivery requires ruthless scope control. Early validation prevents wasted effort on speculative features.

### II. Security by Default
All inputs MUST be treated as potentially hostile. CORS policies MUST be explicitly configured and never left at defaults. URLs and user input MUST be validated and sanitized. HTTP requests MUST use HTTPS in production. Sensitive configuration (API endpoints, secrets) MUST never be checked into source control. Cross-site scripting (XSS) prevention MUST be enforced in Blazor templates. SQL injection prevention MUST be enforced if persistence is added.

**Rationale**: Security vulnerabilities compound in production. Establishing secure defaults early prevents retrofitting later.

### III. Code Quality & Maintainability (NON-NEGOTIABLE)
All production code MUST follow SOLID principles and clean code standards. Methods MUST have single, clear responsibility (maximum ~50 lines). Complex logic MUST be tested with unit tests. All public APIs MUST have XML documentation comments. Code review MUST verify compliance before merge. Naming conventions MUST be clear and consistent (PascalCase for types/methods, camelCase for locals/parameters). Dead code and commented-out code MUST be removed.

**Rationale**: Technical debt multiplies over time. Quality practices now prevent slow decay.

### IV. Separation of Concerns
Backend (ASP.NET Core) MUST handle API contracts, business logic, and (in Extended-MVP) feed operations. Frontend (Blazor WebAssembly) MUST handle UI state, user interaction, and presentation logic. Communication between backend and frontend MUST use well-defined HTTP contracts (JSON). Each layer MUST have clear error responsibilities: backend MUST return appropriate HTTP status codes; frontend MUST present user-friendly error messages.

**Rationale**: Clear boundaries prevent architectural confusion and enable independent testing and scaling.

### V. Incremental Architecture
The technology stack (ASP.NET Core + Blazor) MUST support incremental feature addition without major refactoring. MVP storage (in-memory) MUST NOT couple to the data model, allowing for future database persistence without changing the API contract. Dependencies (e.g., feed parsing libraries) MUST be added only when needed, not speculatively. All architectural changes MUST be justified and documented.

**Rationale**: This prevents over-engineering and keeps delivery velocity high.

## Security Requirements

- **Input Validation**: All incoming data (URLs, form inputs) MUST be validated for type, length, and format before processing.
- **CORS Configuration**: CORS MUST be explicitly configured to allow only known frontend origins. Wildcard settings MUST NOT be used.
- **HTTPS in Production**: All production deployments MUST use HTTPS. HTTP MUST only be used for local development testing.
- **Configuration Management**: API URLs and secrets MUST be stored in environment variables or secure configuration, never hardcoded. `.env` files MUST be added to `.gitignore`.
- **Error Messages**: Error messages returned to the client MUST NOT expose internal implementation details or stack traces. Sensitive information MUST be logged server-side only.
- **XSS Prevention**: Blazor binding MUST use safe property binding; inline HTML rendering MUST be avoided unless explicitly sanitized.

## Code Quality Standards

- **Unit Test Coverage**: All business logic MUST have unit tests. Minimum threshold: 70% coverage for new code.
- **Documentation**: Public APIs, complex algorithms, and configuration MUST be documented. README MUST include build/run instructions and local development checklist.
- **Code Review**: All code MUST be reviewed before merge. Reviewers MUST verify compliance with this constitution.
- **Static Analysis**: Code MUST pass linting and analysis tools (StyleCop, Roslyn analyzers). Warnings MUST be addressed or explicitly suppressed with justification.
- **Git Hygiene**: Commit messages MUST be clear and descriptive. Commits MUST be logical units; avoid mixing unrelated changes.
- **Tech Debt**: Spike issues for known tech debt MUST be filed and tracked. Technical decisions MUST be documented (ADRs recommended).

## Governance

This Constitution supersedes all other development guidelines and practices for the RSS Feed Reader project.

**Amendment Process**: Changes to this constitution MUST be documented, ratified by the project lead, and reflected in a version number bump. All PRs MUST verify compliance before merge. Ambiguities or conflicts MUST be escalated to the project lead for clarification.

**Compliance Verification**: Code reviews MUST check this constitution. Security audits MUST occur before production deployment. Principles MUST be treated as non-negotiable unless explicitly amended.

**Version Semantics**: Changes follow semantic versioning: MAJOR for principle removals or redefinitions, MINOR for new principles or expanded guidance, PATCH for clarifications and wording.

**Version**: 1.0.0 | **Ratified**: 2026-03-08 | **Last Amended**: 2026-03-08
