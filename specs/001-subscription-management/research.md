# Research: MVP Subscription Management - Phase 0

**Objective**: Resolve technical unknowns and design decision conflicts identified in planning phase.

**Input**: Feature spec, stakeholder documents, project constitution.

---

## Research Task 1: Validation Approach (CRITICAL)

**Question**: How to reconcile spec requirement "accept any URL without validation" with Constitution principle "All incoming data MUST be validated"?

### Findings

**Option A: No Validation (as per spec)**
- **Approach**: Frontend and backend accept and store any input without checks
- **Pros**: Fastest implementation, matches spec literally
- **Cons**: Violates Constitution security principle, no protection against malformed input
- **Risk**: User confusion if URL storage fails due to length, encoding issues

**Option B: Light Validation (RECOMMENDED - Chosen)**
- **Approach**: 
  - Frontend: Trim whitespace, enforce reasonable length limit (max 2000 chars), basic string checks
  - Backend: Log suspicious patterns, sanitize before storage
  - No validation of URL format (don't know if it's valid RSS/Atom)
- **Pros**: Balances Constitution security principle with spec intent; prevents obvious problems; aligns with "assume user provides valid URLs" assumption
- **Cons**: Slightly more code than no validation
- **Justification**: Single trusted user POC; not a public service; prevents data corruption; aligns with Constitution intent (security by default, not by absence)

**Option C: Full Validation (too heavy for MVP)**
- **Approach**: URL format validation, attempt to parse as URL, etc.
- **Pros**: Maximum protection
- **Cons**: Violates MVP-First principle (too complex), contradicts stakeholder guidance
- **Rejected Because**: Over-engineering for proof-of-concept

### Decision

**CHOSEN**: Option B - Light Validation (frontend trim/length check + backend logging)

**Rationale**: Satisfies both Constitution (security by default) and spec intent (doesn't validate URLs as RSS/Atom feeds). "No validation" in spec means "no feed validation", not "no input sanitation".

**Implementation Details**:
- Frontend AddSubscriptionForm: Trim input, validate max length 2000 chars, disallow if blank/whitespace-only
- Backend SubscriptionService: Log URLs with suspicious patterns (but accept them), basic string sanitization
- Documentation: Clarify that "no validation" means "assume URLs are valid feeds" not "accept malformed input"

---

## Research Task 2: In-Memory Storage Architecture

**Question**: How to structure in-memory storage to support future database migration without API changes?

### Findings

**Best Practice**: Decouple storage layer from API service layer using dependency injection.

**Approach**:
1. Define `ISubscriptionRepository` interface (abstraction)
2. Implement `InMemorySubscriptionRepository` for MVP (List<string> backend)
3. Inject ISubscriptionRepository into `SubscriptionService`
4. `SubscriptionService` uses interface, not concrete implementation
5. Future: Create `DatabaseSubscriptionRepository` implementing same interface, swap in DI

**Benefits**:
- API contract unchanged when switching storage
- Easy to test with mock repositories
- Follows SOLID principles (Dependency Inversion)
- No "rip and replace" refactoring needed for Extended-MVP

**Implementation**:
```csharp
public interface ISubscriptionRepository
{
    Task<IEnumerable<string>> GetAllAsync();
    Task AddAsync(string url);
}

public class SubscriptionService
{
    private readonly ISubscriptionRepository _repository;
    public SubscriptionService(ISubscriptionRepository repository) => _repository = repository;
    public async Task AddSubscriptionAsync(string url) => await _repository.AddAsync(url);
}
```

### Decision

**CHOSEN**: ISubscriptionRepository abstraction with InMemorySubscriptionRepository implementation

---

## Research Task 3: Blazor Component Architecture for MVP

**Question**: How to structure Blazor components to keep subscription management simple but testable?

### Findings

**Recommended Structure**:
1. **Subscriptions.razor** (Page component) - Main page with `@page "/"`, orchestrates layout
2. **AddSubscriptionForm.razor** (Component) - Encapsulates form logic, emits event when submitted
3. **SubscriptionList.razor** (Component) - Displays list, optionally allows deletion (future)
4. **SubscriptionApiClient.cs** (Service) - HTTP client for backend communication

**State Management**:
- Parent component (Subscriptions.razor) holds state (List<string>)
- Child components receive state via parameters
- Child components emit callbacks to parent for updates
- Parent refreshes list after each add operation

**Simplification for MVP**:
- No complex state management (Xaml/OOP patterns)
- Direct API calls to backend (no polling, no WebSocket)
- Immediate feedback via list re-render

### Decision

**CHOSEN**: Simple parent-child component structure with parent state management

---

## Research Task 4: CORS Configuration for MVP

**Question**: How to configure CORS for development and ensure it aligns with Constitution security principle?

### Findings

**Development Setup**:
- Backend runs on localhost:5001 (HTTPS) or localhost:5000 (HTTP)
- Frontend runs on localhost:5173 (Blazor dev server)
- CORS policy must explicitly allow localhost:5173 origin

**Constitution Requirement**: "CORS MUST be explicitly configured to allow only known frontend origins. Wildcard settings MUST NOT be used."

**Recommended Approach**:
```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy
            .WithOrigins("https://localhost:5173", "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
```

**Non-Production Enforcement**:
- Use environment-specific CORS policies
- Development: Explicit localhost origins
- Production: Explicit deployed frontend URL (when applicable)
- Never use wildcard `AllowAnyOrigin()`

### Decision

**CHOSEN**: Explicit CORS policy per environment, never wildcard

---

## Research Task 5: Testing Strategy for MVP

**Question**: What testing approach balances MVP speed with Constitution code quality requirements?

### Findings

**Constitution Requirement**: "All business logic MUST have unit tests. Minimum threshold: 70% coverage for new code."

**MVP Testing Approach**:
1. **Unit Tests** (Primary):
   - SubscriptionService business logic (add, get)
   - Tests: valid URL storage, duplicate handling, empty list behavior
   - Framework: xUnit
   - Target: 80%+ coverage on service layer
   
2. **Component Tests** (Secondary):
   - AddSubscriptionForm interaction tests
   - SubscriptionList rendering tests
   - Framework: Blazor component testing libraries
   - Scope: Happy paths only, not extensive
   
3. **Integration Tests** (Minimal MVP):
   - One end-to-end test: Add subscription → See in list
   - Not comprehensive (Extended-MVP expands this)

4. **No E2E Tests** (out of scope for MVP):
   - Too slow for rapid development
   - Limited test coverage relative to time investment

### Decision

**CHOSEN**: Unit tests (70%+ coverage) + component tests (happy paths) + 1 minimal integration test

---

## Research Task 6: Error Handling & User Feedback

**Question**: What error handling strategy aligns with spec (no error handling) and Constitution (clear error responsibilities)?

### Findings

**Spec Requirement**: "No error handling needed (no network operations)" - Implies happy path only for MVP

**Constitution Requirement**: "Each layer MUST have clear error responsibilities: backend MUST return appropriate HTTP status codes; frontend MUST present user-friendly error messages."

**Balanced Approach**:
1. **Happy Path Priority**: Design for success (valid URLs, HTTP up)
2. **Defensive Coding**: Null checks, bounds checks, basic guards
3. **Logging**: Backend logs errors for debugging (not shown to user)
4. **User Feedback**: 
   - Success: List updates immediately
   - Network error: Show generic "Unable to connect" message (one line)
   - No detailed error messages for MVP

**Implementation**:
- Backend HttpStatusCode 200 for success, 500 for errors (minimal)
- Frontend catches HTTP errors, shows toast "Failed to add subscription. Please try again."
- No stack traces or detailed diagnostics displayed

### Decision

**CHOSEN**: Happy path focus + basic error recovery without detailed diagnostics

---

## Summary of Resolutions

| Conflict/Unknown | Decision | Justification |
|------------------|----------|---------------|
| Validation approach | Light validation (trim, length check) | Balances Constitution security with spec intent |
| Storage abstraction | ISubscriptionRepository interface | Enables future DB migration without API changes |
| Component structure | Parent-child with parent state | Simple, testable, no complex patterns |
| CORS policy | Explicit per-environment, never wildcard | Aligns with Constitution security principle |
| Testing strategy | Unit (70%+) + component + 1 integration | Balances quality with MVP speed |
| Error handling | Happy path + basic recovery, no details | Matches spec but respects Constitution error responsibilities |

**All research findings resolved.** No outstanding "NEEDS CLARIFICATION" markers remain.
