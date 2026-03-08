---
description: "Task list for MVP Subscription Management implementation"
---

# Tasks: MVP Subscription Management

**Input**: Design documents from `/specs/001-subscription-management/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/api-contract.md ✅

**Feature**: Subscription Management — Add and display RSS/Atom feed subscriptions
**Branch**: `001-subscription-management`
**Created**: 2026-03-08

**Notes**:
- Tasks organized by user story to enable independent implementation
- All tasks follow checklist format: `- [ ] [T###] [P?] [Story?] Description with file path`
- [P] marker indicates parallelizable tasks (different files, no dependencies)
- [Story] label identifies which user story the task belongs to (US1, US2)
- Estimated total: 55 tasks across 5 phases

---

## Phase 1: Setup (Project Initialization)

**Purpose**: Initialize project structure, create solutions, and configure base infrastructure

**Duration**: ~30-45 minutes

- [ ] T001 Create solution and project structure: `dotnet new sln -n RSSFeedReader` at repository root
- [ ] T002 Create backend API project: `dotnet new webapi -n backend/RSSFeedReader.API -f net8.0`
- [ ] T003 Create backend test project: `dotnet new xunit -n backend/RSSFeedReader.API.Tests -f net8.0`
- [ ] T004 Create frontend Blazor project: `dotnet new blazorwasm -n frontend/RSSFeedReader.UI -f net8.0`
- [ ] T005 Create frontend test project: `dotnet new xunit -n frontend/RSSFeedReader.UI.Tests -f net8.0`
- [ ] T006 [P] Add projects to solution: `dotnet sln RSSFeedReader.sln add` for all four projects
- [ ] T007 Create `.gitignore` for backend project: exclude bin/, obj/, *.user, appsettings.local.json
- [ ] T008 [P] Create `.gitignore` for frontend project: exclude bin/, obj/, node_modules/ (if applicable)
- [ ] T009 Add xUnit and testing NuGet packages: `Moq`, `FluentAssertions` to both test projects
- [ ] T010 Verify solution builds: `dotnet build RSSFeedReader.sln` succeeds without errors

**Checkpoint**: All projects created, solution builds successfully

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that blocks all user story work until complete

**Critical**: No user story work begins until this phase is complete

**Duration**: ~2-3 hours

### Backend Infrastructure

- [ ] T011 [P] Create backend Models directory structure: `backend/src/RSSFeedReader.API/Models/`
- [ ] T012 [P] Create backend Services directory structure: `backend/src/RSSFeedReader.API/Services/`
- [ ] T013 Create base Subscription model in `backend/RSSFeedReader.API/Models/Subscription.cs` with properties: Id (int), Url (string), AddedAt (DateTime)
- [ ] T014 Create AddSubscriptionRequest DTO in `backend/RSSFeedReader.API/Models/AddSubscriptionRequest.cs` with [Required] and [StringLength(2000, MinimumLength = 1)] validation
- [ ] T015 Create SubscriptionDto in `backend/RSSFeedReader.API/Models/SubscriptionDto.cs` (same as Subscription for responses)
- [ ] T016 Create SubscriptionsResponse in `backend/RSSFeedReader.API/Models/SubscriptionsResponse.cs` with `IEnumerable<SubscriptionDto> Subscriptions` property
- [ ] T017 Create ISubscriptionRepository interface in `backend/RSSFeedReader.API/Services/ISubscriptionRepository.cs` with GetAllAsync() and AddAsync(string url) methods
- [ ] T018 Create InMemorySubscriptionRepository in `backend/RSSFeedReader.API/Services/InMemorySubscriptionRepository.cs` implementing ISubscriptionRepository with static List<Subscription>
- [ ] T019 Add XML documentation comments to InMemorySubscriptionRepository explaining static collection and session-only persistence
- [ ] T020 Create SubscriptionService in `backend/RSSFeedReader.API/Services/SubscriptionService.cs` injecting ISubscriptionRepository
- [ ] T021 Implement SubscriptionService.GetAllAsync() method delegating to repository with no transformation
- [ ] T022 Implement SubscriptionService.AddAsync(string url) method: trim URL, validate max 2000 chars, call repository.AddAsync()

### Backend Configuration

- [ ] T023 Update `backend/RSSFeedReader.API/Program.cs` to add CORS policy named "DevelopmentPolicy"
- [ ] T024 Configure CORS in Program.cs to allow https://localhost:5173 and http://localhost:5173 origins
- [ ] T025 Register dependency injection in Program.cs: `services.AddScoped<ISubscriptionRepository, InMemorySubscriptionRepository>()`
- [ ] T026 Register SubscriptionService in DI: `services.AddScoped<SubscriptionService>`
- [ ] T027 Add CORS middleware to request pipeline: `app.UseCors("DevelopmentPolicy")` in Program.cs
- [ ] T028 Test backend builds without errors: `dotnet build backend/RSSFeedReader.API`

### Frontend Infrastructure

- [ ] T029 [P] Create frontend Models directory: `frontend/src/RSSFeedReader.UI/Models/`
- [ ] T030 [P] Create frontend Services directory: `frontend/src/RSSFeedReader.UI/Services/`
- [ ] T031 [P] Create frontend Pages directory: `frontend/src/RSSFeedReader.UI/Pages/`
- [ ] T032 [P] Create frontend Components directory: `frontend/src/RSSFeedReader.UI/Components/`
- [ ] T033 Copy AddSubscriptionRequest DTO to frontend: `frontend/src/RSSFeedReader.UI/Models/AddSubscriptionRequest.cs`
- [ ] T034 Copy SubscriptionDto to frontend: `frontend/src/RSSFeedReader.UI/Models/SubscriptionDto.cs`
- [ ] T035 Review and update `frontend/src/RSSFeedReader.UI/wwwroot/appsettings.json` to include `"ApiUrl": "http://localhost:5000"`
- [ ] T036 Test frontend builds: `dotnet build frontend/RSSFeedReader.UI` succeeds without errors

### Local Development Checklist Documentation

- [ ] T037 Create `docs/local-dev-checklist.md` with pre-launch verification steps (from quickstart.md)
- [ ] T038 Document backend startup command in checklist
- [ ] T039 Document frontend startup command in checklist

**Checkpoint**: All foundational infrastructure in place, both backend and frontend build successfully. DI configured. Ready for user story implementation.

---

## Phase 3: User Story 1 - Add Feed Subscription (Priority: P1)

**Goal**: Enable users to enter a feed URL and have it added to their subscription list

**Independent Test**: User enters URL, clicks Add, sees URL appear in list (this story can be tested independently, but US2 completion is needed for visual feedback)

**Duration**: ~4-5 hours

### Backend Implementation - User Story 1

- [ ] T040 [P] [US1] Create SubscriptionsController in `backend/RSSFeedReader.API/Controllers/SubscriptionsController.cs` inheriting from ControllerBase
- [ ] T041 [US1] Add [ApiController] and [Route("api/[controller]")] attributes to controller
- [ ] T042 [P] [US1] Inject SubscriptionService into SubscriptionsController constructor
- [ ] T043 [US1] Implement POST /api/subscriptions endpoint accepting AddSubscriptionRequest
- [ ] T044 [US1] Add validation in POST endpoint: check if URL is null/empty after trimming, return 400 Bad Request if invalid
- [ ] T045 [US1] Add validation in POST endpoint: check if URL exceeds 2000 chars, return 400 Bad Request if too long
- [ ] T046 [US1] Implement successful POST response: return 201 Created with created subscription DTO
- [ ] T047 [US1] Add [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status201Created)] to POST endpoint
- [ ] T048 [US1] Add [ProducesResponseType(StatusCodes.Status400BadRequest)] to POST endpoint
- [ ] T049 [US1] Add XML documentation comment to POST endpoint describing the operation and response codes

### Backend Testing - User Story 1

- [ ] T050 [P] [US1] Create SubscriptionServiceTests in `backend/RSSFeedReader.API.Tests/Services/SubscriptionServiceTests.cs`
- [ ] T051 [US1] Write unit test: AddAsync stores URL exactly as provided (without trimming special chars)
- [ ] T052 [US1] Write unit test: AddAsync with URL at max length (2000 chars) succeeds
- [ ] T053 [US1] Write unit test: AddAsync with URL > 2000 chars throws ArgumentException
- [ ] T054 [US1] Write unit test: AddAsync with whitespace-only input returns success (MVP design allows this)
- [ ] T055 [US1] Write unit test: Multiple AddAsync calls add subscriptions without collision
- [ ] T056 [P] [US1] Create SubscriptionsControllerTests in `backend/RSSFeedReader.API.Tests/Controllers/SubscriptionsControllerTests.cs`
- [ ] T057 [US1] Write integration test: POST /api/subscriptions with valid URL returns 201 with subscription DTO
- [ ] T058 [US1] Write integration test: POST /api/subscriptions with empty URL returns 400 Bad Request
- [ ] T059 [US1] Write integration test: POST /api/subscriptions with URL > 2000 chars returns 400 Bad Request
- [ ] T060 [US1] Verify unit test coverage >= 70% in SubscriptionService and Controller classes
- [ ] T061 [US1] Run all backend tests: `dotnet test backend/` (all tests PASS)

**Checkpoint**: Backend can accept POST requests to add subscriptions, validates inputs, responds with correct status codes and DTOs. All tests passing.

---

## Phase 4: User Story 2 - Display Subscription List (Priority: P1)

**Goal**: Display all currently stored subscriptions as a list in the UI, updating immediately after additions

**Independent Test**: Load app, add subscriptions, verify list displays in correct order

**Duration**: ~5-6 hours

### Backend Implementation - User Story 2

- [ ] T062 [P] [US2] Implement GET /api/subscriptions endpoint in SubscriptionsController
- [ ] T063 [US2] GET endpoint returns 200 OK with SubscriptionsResponse containing all subscriptions
- [ ] T064 [US2] GET endpoint returns subscriptions in order they were added (by AddedAt timestamp or insertion order)
- [ ] T065 [US2] Add [ProducesResponseType(typeof(SubscriptionsResponse), StatusCodes.Status200OK)] to GET endpoint
- [ ] T066 [US2] Add [ProducesResponseType(StatusCodes.Status500InternalServerError)] to GET endpoint
- [ ] T067 [US2] Add XML documentation to GET endpoint describing the operation and response

### Backend Testing - User Story 2

- [ ] T068 [P] [US2] Write integration test: GET /api/subscriptions returns empty subscriptions list initially
- [ ] T069 [US2] Write integration test: POST subscription then GET /api/subscriptions returns subscription in response
- [ ] T070 [US2] Write integration test: Add 3 subscriptions, GET returns all 3 in order added
- [ ] T071 [US2] Write integration test: GET /api/subscriptions returns 200 OK status
- [ ] T072 [US2] Verify unit test coverage >= 70% for GET endpoint
- [ ] T073 [US2] Run backend tests including GET endpoint tests (all tests PASS)

### Frontend Implementation - User Story 2

- [ ] T074 [P] [US2] Create SubscriptionApiClient service in `frontend/src/RSSFeedReader.UI/Services/SubscriptionApiClient.cs`
- [ ] T075 [US2] Inject HttpClient into SubscriptionApiClient via constructor
- [ ] T076 [US2] Read ApiUrl from appsettings.json in SubscriptionApiClient constructor
- [ ] T077 [US2] Implement GetSubscriptionsAsync() method calling GET /api/subscriptions endpoint
- [ ] T078 [US2] Implement AddSubscriptionAsync(string url) method calling POST /api/subscriptions endpoint
- [ ] T079 [US2] Add error handling to both methods: log errors, return empty list or throw on failure
- [ ] T080 [US2] Add XML documentation to SubscriptionApiClient methods

### Frontend Components - User Story 2

- [ ] T081 [P] [US2] Create SubscriptionList component in `frontend/src/RSSFeedReader.UI/Components/SubscriptionList.razor`
- [ ] T082 [US2] SubscriptionList receives `IEnumerable<SubscriptionDto>` subscriptions as parameter
- [ ] T083 [US2] Render subscriptions as HTML list (ul/li or table format)
- [ ] T084 [US2] Display "No subscriptions" message when list is empty
- [ ] T085 [US2] Display each subscription URL, AddedAt timestamp in list
- [ ] T086 [US2] Order subscriptions by AddedAt (oldest first) in component
- [ ] T087 [P] [US2] Create AddSubscriptionForm component in `frontend/src/RSSFeedReader.UI/Components/AddSubscriptionForm.razor`
- [ ] T088 [US2] AddSubscriptionForm has input field for URL, pre-filled with empty string
- [ ] T089 [US2] AddSubscriptionForm has "Add Subscription" button
- [ ] T090 [US2] AddSubscriptionForm validates input: reject if empty or > 2000 chars
- [ ] T091 [US2] AddSubscriptionForm emits OnSubscriptionAdded callback when form submitted successfully
- [ ] T092 [US2] AddSubscriptionForm shows validation error message if URL invalid
- [ ] T093 [US2] AddSubscriptionForm clears input field after successful submission

### Frontend Pages - User Story 2

- [ ] T094 [US2] Create Subscriptions.razor page in `frontend/src/RSSFeedReader.UI/Pages/Subscriptions.razor` with route `@page "/"`
- [ ] T095 [US2] Subscriptions page uses `@page "/"` directive (main landing page)
- [ ] T096 [US2] Subscriptions page injects SubscriptionApiClient service
- [ ] T097 [US2] OnInitializedAsync in page loads subscriptions via GetSubscriptionsAsync() on page load
- [ ] T098 [US2] Page displays AddSubscriptionForm and SubscriptionList components
- [ ] T099 [US2] Page handles AddSubscriptionForm's OnSubscriptionAdded callback: add URL via API, refresh list
- [ ] T100 [US2] Page updates SubscriptionList component when subscription added (list updates immediately <100ms)
- [ ] T101 [US2] Add error handling: show user-friendly error message if API call fails
- [ ] T102 [US2] Add loading state: show "Loading..." during initial page load

### Frontend Template Cleanup (CRITICAL per TechStack doc)

- [ ] T103 [US2] Delete template demo pages from Pages directory: Home.razor, Counter.razor, Weather.razor
- [ ] T104 [US2] Update NavMenu.razor: remove demo navigation links, update to show only "Subscriptions"
- [ ] T105 [US2] Verify only Subscriptions.razor uses `@page "/"` directive
- [ ] T106 [US2] Verify no route conflicts in remaining pages
- [ ] T107 [US2] Verify frontend builds without errors after template cleanup

### Frontend Testing - User Story 2

- [ ] T108 [P] [US2] Create SubscriptionListTests component tests in `frontend/src/RSSFeedReader.UI.Tests/Components/SubscriptionListTests.cs`
- [ ] T109 [US2] Write component test: SubscriptionList renders empty message when no subscriptions
- [ ] T110 [US2] Write component test: SubscriptionList renders all subscriptions when provided
- [ ] T111 [US2] Write component test: SubscriptionList displays subscriptions in correct order
- [ ] T112 [P] [US2] Create AddSubscriptionFormTests in `frontend/src/RSSFeedReader.UI.Tests/Components/AddSubscriptionFormTests.cs`
- [ ] T113 [US2] Write component test: AddSubscriptionForm rejects empty input with validation error
- [ ] T114 [US2] Write component test: AddSubscriptionForm rejects URL > 2000 chars with validation error
- [ ] T115 [US2] Write component test: AddSubscriptionForm accepts valid URL and fires OnSubscriptionAdded callback
- [ ] T116 [US2] Write component test: AddSubscriptionForm clears input after successful submission
- [ ] T117 [US2] Write API client test: GetSubscriptionsAsync() calls GET /api/subscriptions endpoint
- [ ] T118 [US2] Write API client test: AddSubscriptionAsync() calls POST /api/subscriptions endpoint
- [ ] T119 [US2] Verify test coverage >= 70% for component and service code
- [ ] T120 [US2] Run frontend tests: `dotnet test frontend/` (all tests PASS)

**Checkpoint**: Frontend displays subscription list, form accepts user input, list updates immediately after adding subscription. All tests passing. Template cleanup complete.

---

## Phase 5: Integration & End-to-End Testing

**Purpose**: Verify backend and frontend work together correctly in complete user scenarios

**Duration**: ~2-3 hours

### Integration Testing

- [ ] T121 [P] Start backend: `dotnet run` in `backend/RSSFeedReader.API` (listens on https://localhost:5001 or http://localhost:5000)
- [ ] T122 [P] Start frontend: `dotnet run` in `frontend/src/RSSFeedReader.UI` (listens on https://localhost:5173)
- [ ] T123 Open browser to https://localhost:5173, page loads without console errors
- [ ] T124 Verify browser DevTools Network tab: no connection errors to API
- [ ] T125 Enter feed URL in form: "https://devblogs.microsoft.com/dotnet/feed/"
- [ ] T126 Click "Add Subscription", verify URL appears in list within 1 second
- [ ] T127 Add 3 more subscriptions, verify all appear in list in order added
- [ ] T128 Refresh browser page (F5), verify subscriptions are lost (in-memory, as designed)
- [ ] T129 Verify API returns correct HTTP status codes: 201 for POST, 200 for GET, 400 for bad input
- [ ] T130 Verify error handling: try adding empty URL, see validation error message (no stack trace)

### End-to-End Test Suite

- [ ] T131 Create end-to-end test: Add subscription then retrieve list via API (verify in list)
- [ ] T132 Create end-to-end test: Add 10 subscriptions without errors or performance degradation
- [ ] T133 Create end-to-end test: Long URL with query parameters stored and displayed exactly
- [ ] T134 Create end-to-end test: Add duplicate URL (verify both appear in list)
- [ ] T135 Document test scenarios in `docs/end-to-end-tests.md`

### Local Development Verification

- [ ] T136 Verify [local-dev-checklist.md](../../../docs/local-dev-checklist.md) completeness
- [ ] T137 All checklist items in local-dev-checklist passing (✓)

**Checkpoint**: Complete MVP working end-to-end. Users can add subscriptions and see them in the list.

---

## Phase 6: Polish & Documentation

**Purpose**: Final quality checks, documentation, and preparation for release

**Duration**: ~2-3 hours

### Code Quality & Cleanup

- [ ] T138 Run code formatter: `dotnet format` on entire solution
- [ ] T139 Verify C# style compliance: no StyleCop warnings
- [ ] T140 Review all public APIs: verify XML documentation is complete
- [ ] T141 Remove any commented-out code from all files
- [ ] T142 Remove debug logging, Console.WriteLine() statements
- [ ] T143 Verify no hardcoded URLs or config values (use appsettings.json and environment variables)

### Documentation

- [ ] T144 Update `README.md` with MVP feature description, setup instructions, running guide
- [ ] T145 Update README with links to design documents (plan.md, data-model.md, api-contract.md)
- [ ] T146 Create `docs/architecture.md` describing backend/frontend separation, API contract, storage layer
- [ ] T147 Document CORS configuration in `docs/architecture.md`
- [ ] T148 Document repository pattern and future database migration in `docs/architecture.md`
- [ ] T149 Create CONTRIBUTING.md with code style guidelines, testing requirements, PR checklist

### Testing Coverage

- [ ] T150 Run full test suite with coverage report: `dotnet test /p:CollectCoverage=true`
- [ ] T151 Verify backend code coverage >= 70% (focus on services and controllers)
- [ ] T152 Verify frontend code coverage >= 70% (focus on services and components)
- [ ] T153 Fix any coverage gaps identified

### Git & Repository

- [ ] T154 Stage all files: `git add .`
- [ ] T155 Commit with message: "feat: implement MVP subscription management (add, display)"
- [ ] T156 Verify commit message explains what feature is delivered (add/display subscriptions)
- [ ] T157 Push to feature branch: `git push origin 001-subscription-management`

### Pre-Release Verification

- [ ] T158 Run complete build: `dotnet build RSSFeedReader.sln` (no errors)
- [ ] T159 Run all tests: `dotnet test` (all tests PASS, coverage >= 70%)
- [ ] T160 Verify API returns correct status codes (200, 201, 400, 500) for all scenarios
- [ ] T161 Manual verification: E2E flow works perfectly (add 5 subscriptions, see in list)
- [ ] T162 Verify no security issues: no SQL injection (not applicable, in-memory), no XSS, CORS correct

**Checkpoint**: MVP complete, tested, documented, code quality verified. Ready for extended-MVP phase.

---

## Task Organization Summary

| Phase | Task Range | Count | Status | Duration |
|-------|-----------|-------|--------|----------|
| Phase 1: Setup | T001-T010 | 10 | ⏳ Not Started | 30-45 min |
| Phase 2: Foundational | T011-T039 | 29 | ⏳ Not Started | 2-3 hours |
| Phase 3: User Story 1 (US1) | T040-T061 | 22 | ⏳ Not Started | 4-5 hours |
| Phase 4: User Story 2 (US2) | T062-T120 | 59 | ⏳ Not Started | 5-6 hours |
| Phase 5: Integration & E2E | T121-T137 | 17 | ⏳ Not Started | 2-3 hours |
| Phase 6: Polish & Docs | T138-T162 | 25 | ⏳ Not Started | 2-3 hours |
| **TOTAL** | **T001-T162** | **162** | **⏳ Not Started** | **~16-24 hours** |

---

## Parallelization Opportunities

### Parallel Setup (Phase 1)
- T006, T007, T008 can run in parallel (adding projects to solution, creating .gitignore files)
- T009 can run once all projects created

### Parallel Foundational (Phase 2)
- **Backend models** (T011, T012, T013-T022) can be done in parallel with **Backend config** (T023-T028)
- **Frontend structure** (T029-T032) can be done in parallel with backend work
- **Frontend models** (T033-T035) can begin once backend models defined

### Parallel User Story Work (Phase 3 & 4)
- **Backend implementation** (T040-T049, T062-T067) can progress independently of **Frontend** (T074-T120)
- **Backend tests** (T050-T061) can run parallel with **Frontend components** (T081-T093)
- **Frontend template cleanup** (T103-T107) can happen parallel with component development

### Example Parallel Execution Flow

**Fast Path (Recommended Order for Speed)**:
1. Setup Phase: ~45 min (linear, must complete first)
2. Parallel tracks for Phase 2:
   - Track A: Backend models & services (T011-T022)
   - Track B: Frontend structure & models (T029-T035)
   - Merge: Backend config + Frontend config (T023-T028, T036, T037)
3. Parallel tracks for Phase 3-4:
   - Track A: Backend implementation & tests (T040-T073)
   - Track B: Frontend components, pages, cleanup (T074-T120)
   - Merge: Integration testing (T121-T135)
4. Polish & Release (T136-T162): ~3 hours

**Estimated Total Time with Parallelization**: 10-15 hours (instead of 16-24 hours sequential)

---

## User Story Completion Order

**All tasks organized by user story enable independent implementation**:

✅ **User Story 1 (Add Feed Subscription)**: INDEPENDENT  
- Can test by adding subscriptions via POST API endpoint
- Can verify with unit tests and integration tests
- Does not require frontend or display (US2) to be complete
- Completion: T061 backend tests pass

✅ **User Story 2 (Display Subscription List)**: DEPENDENT on US1  
- Requires US1 backend to be working (POST endpoint complete)
- Adds frontend and GET endpoint
- Completion: T120 all frontend tests pass + T135 E2E tests pass

**Both stories are P1 and should complete roughly in parallel after Phase 2 foundational work.**

---

## Success Metrics

**Phase 1 Complete**: All projects created, solution builds
**Phase 2 Complete**: Infrastructure ready, DI configured, both projects compile
**Phase 3 Complete**: Backend can accept POST requests, all backend tests pass (70%+ coverage)
**Phase 4 Complete**: Frontend displays list, E2E test passes (add → see in list), all frontend tests pass (70%+ coverage)
**Phase 5 Complete**: Manual testing successful, users can demo MVP
**Phase 6 Complete**: Code quality verified, documentation complete, ready for extended-MVP

---

## Notes for Implementation

1. **Test Coverage**: Minimum 70% required per Constitution. Focus on SubscriptionService and Controllers first.
2. **Template Cleanup**: CRITICAL per TechStack document. Must be complete before feature demo (Phase 4, T103-T107).
3. **CORS Configuration**: Explicit per-environment (never wildcard). Document which origins are allowed.
4. **Validation Decision**: Light validation (trim + length check) implemented as per research.md findings. Not zero validation, but accepts "no format validation" requirement.
5. **Error Handling**: Happy path priority. Errors logged server-side, user sees generic messages (no stack traces).
6. **Repository Pattern**: Enables future database migration without API changes. Design abstraction carefully.
7. **In-Memory Storage**: Static collection is acceptable for MVP. Document that subscriptions lost on restart by design.
8. **Performance Goals**: API <500ms, UI <100ms. Monitor during manual testing.
9. **Git Commits**: Use conventional commit messages (feat:, fix:, test:, docs:, refactor:).
10. **Documentation**: Link all design documents (plan.md, data-model.md, api-contract.md) in README and architecture docs.

---

## Definition of Done (MVP Complete)

✅ All Phase 1-5 tasks complete  
✅ All Phase 6 tasks complete  
✅ All tests passing (70%+ coverage)  
✅ Manual E2E verification: Add subscription → see in list works correctly  
✅ Code formatted and linted  
✅ Documentation complete and linked  
✅ Git committed with meaningful messages  
✅ No compiler warnings or errors  
✅ CORS configured correctly  
✅ No hardcoded URLs or secrets  

**MVP Feature Complete**: Users can add RSS/Atom feed subscriptions and view their subscription list in-memory with immediate visual feedback.
