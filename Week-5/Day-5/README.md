<div align="center">

# Day 5 — Applying Testing to the Chosen Project & Week 5 Synthesis

*Field notes from the day everything from Week 5 got pointed at the actual project — and Week 5 closed out.*

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-Unit%20%2B%20Integration-5E5E5E?logo=xunit&logoColor=white)
![Moq](https://img.shields.io/badge/Moq-Mocking-6B4FBB?logo=.net&logoColor=white)
![WebApplicationFactory](https://img.shields.io/badge/Mvc.Testing-WebApplicationFactory-512BD4?logo=dotnet&logoColor=white)
![Status](https://img.shields.io/badge/status-complete-2ea44c)

`⏱ 8 hours` · `🧪 38 tests passing` · `📌 Week 5 Synthesis`

</div>

---

## 📌 Today in one sentence

Applying everything from Week 5 to the real project — not writing tests for the sake of a number, but identifying the highest-risk parts of the **Cardiac Patient Monitoring API**, testing them deliberately, running the full suite, and closing out Week 5 as a stable foundation for Phase 3.

## 📌 Learning objectives

- Prioritize what to test first on a project, rather than testing everything equally
- Run a full test suite and interpret its results
- Understand how the Phase 3 sprint structure builds directly on this week's foundation

## 📌 Key topics

- What to test first
- Running the full test suite
- Previewing the Phase 3 sprint structure
- Week 5 synthesis and handoff to Sprint 1

## 📌 What I learned

### 1. What to test first

Testing priority should follow risk and complexity, not just what's easiest to test. Business logic with real branching, anything handling money or authentication, and any code that's already caused a bug once deserve priority. Simple pass-through code with no real logic of its own is lower priority — testing it thoroughly adds little protection relative to the effort.

The question isn't *"did I test every method?"* — it's *"which parts of my application are most likely to cause serious problems if they contain a bug?"*

> **Note to self:** test based on risk and complexity, not simply based on how much code exists.

### 2. Reusing the project instead of starting over

Day 5 explicitly allows reusing the Week 1–4 project instead of building a new skeleton. The Cardiac Patient Monitoring API already had ASP.NET Core Web API, EF Core, SQL Server, ASP.NET Core Identity, JWT authentication, role-based authorization, controllers, services, DTOs, entities, validation, global exception handling, unit tests, and integration tests in place — so Day 5 continued testing the existing application rather than starting fresh.

### 3. Identifying the three highest-risk areas

For this project, three areas stood out as highest risk:

**🔴 Authentication & JWT** — controls who can log in, who can register, what role a user has, what claims land inside the JWT, and whether unauthorized users are rejected. A bug here is a security problem, not just an application bug.

**🔴 Patient Management** — patients are the central data of a healthcare API. The API needs to correctly retrieve, create, update, and delete patients, handle missing patients or users, and respect authorization.

**🟠 Business Logic** — heart rate validation and the `PatientService`'s use of a mocked repository, both containing real branching logic worth protecting.

### 4. Running the full test suite

`dotnet test` runs every test across the unit and integration test projects and reports a pass/fail summary — the same command that will run automatically in Week 9's GitHub Actions CI pipeline on every push. Getting comfortable running the full suite locally and reading its output now means CI failures in Week 9 will already be familiar, not confusing.

```powershell
dotnet test
```

```text
Test summary:
total: 38
failed: 0
succeeded: 38
skipped: 0
duration: 7.2s

Build succeeded
```

### 5. Unit testing vs. integration testing, side by side

**Unit tests** isolate one piece of logic — `HeartRateService` tested directly, or `PatientService` tested against a mocked `IPatientRepository`. Tools: xUnit, Moq. Goal: test one unit of behavior in isolation.

**Integration tests** check whether multiple parts of the application work together. `CustomWebApplicationFactory` hosts a test version of the app using an EF Core InMemory database instead of SQL Server, with test JWT settings (`TestSecretKey`, `TestIssuer`, `TestAudience`) and a seeded test patient (`ID: 1, Ahmad Hassan`) for predictable data. `PatientsApiIntegrationTests` then sends real HTTP requests through `HttpClient`:

```text
HttpClient
    ↓
ASP.NET Core
    ↓
Authentication
    ↓
JWT validation
    ↓
Authorization
    ↓
Controller
    ↓
EF Core
    ↓
InMemory Database
    ↓
HTTP Response
```

A `TestAuthHandler` also provides a fake authenticated user (`NameIdentifier`, `Name`, `Role = Admin`) for testing authorization behavior without a real login flow.

### 6. Previewing the Phase 3 sprint structure

Starting next week, Phase 3 runs as four one-week sprints, each following the same cycle: Sprint Planning on Day 1, daily stand-ups, a mid-sprint mentor code review, and a Sprint Review plus Retrospective on Day 5. Sprint 1 takes the chosen project from database schema and core routes through a working baseline; Sprints 2–4 add authentication and RBAC, performance and caching, and finally testing, documentation, and deployment — converging on the full professional baseline by Week 9.

### 7. Week 5 synthesis and handoff to Sprint 1

Everything built this week — the testing patterns, the centralized error handling — becomes the standard every endpoint built for the capstone project is expected to meet from here forward. A project that skips testing discipline this week doesn't just look worse later; it actively slows down every subsequent sprint, since a bug caught by a test today is far cheaper than the same bug discovered in Week 9's Definition of Done audit.

> **Note to self:** resist the urge to chase 100% test coverage. A smaller set of well-targeted tests covering real risk is more valuable than a large number of shallow tests that mostly check trivial getters and setters.

## 📌 What I built — hands-on lab

- [x] Reused the Cardiac Patient Monitoring API from Weeks 1–4 instead of building a new skeleton
- [x] Identified the 3 highest-risk areas: Authentication & JWT, Patient Management, Business Logic
- [x] Wrote/verified `AuthControllerTests.cs` and `JwtServiceTests.cs` covering registration, login, `/me`, token generation, claims, roles, and missing-config failures
- [x] Wrote/verified `PatientsControllerTests.cs` covering get, get-by-id, my-profile, create, update, and delete — including all key failure paths
- [x] Wrote/verified `HeartRateServiceTests.cs` covering heart rate validation with `[Theory]`-driven test cases
- [x] Wrote/verified `PatientServiceTests.cs` using Moq to mock `IPatientRepository`
- [x] Wrote 3 integration tests in `PatientsApiIntegrationTests.cs` (more than the required 2) using `CustomWebApplicationFactory`
- [x] Verified the test JWT and `TestAuthHandler` correctly simulate an authenticated Admin user
- [x] Ran the full suite with `dotnet test` and confirmed **38/38 tests passing**

**Tools:** xUnit · Moq · Microsoft.AspNetCore.Mvc.Testing · Entity Framework Core InMemory · JWT Authentication

## 📌 Tests by risk area

### 🔴 Authentication & JWT

```text
AuthControllerTests.cs
├── Register_ShouldReturnOk_WhenUserIsCreated
├── Register_ShouldReturnConflict_WhenEmailAlreadyExists
├── Register_ShouldReturnBadRequest_WhenRoleIsInvalid
├── Login_ShouldReturnOk_WhenCredentialsAreValid
├── Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid
├── Me_ShouldReturnUserInformation_WhenUserIsAuthenticated
└── Me_ShouldReturnUnauthorized_WhenUserDoesNotExist

JwtServiceTests.cs
├── GenerateToken_ShouldReturnValidToken
├── GenerateToken_ShouldContainUserClaims
├── GenerateToken_ShouldContainUserRole
├── GenerateToken_ShouldContainMultipleRoles
├── GenerateToken_ShouldThrowException_WhenIssuerIsMissing
└── GenerateToken_ShouldThrowException_WhenSecretKeyIsMissing
```

### 🔴 Patient Management

```text
PatientsControllerTests.cs
├── GetPatients_ShouldReturnAllPatients
├── GetPatient_ShouldReturnPatient_WhenPatientExists
├── GetPatient_ShouldReturnNotFound_WhenPatientDoesNotExist
├── GetMyPatientProfile_ShouldReturnPatient_WhenProfileIsLinked
├── GetMyPatientProfile_ShouldReturnNotFound_WhenProfileIsNotLinked
├── GetMyPatientProfile_ShouldReturnUnauthorized_WhenUserDoesNotExist
├── CreatePatient_ShouldCreatePatient
├── UpdatePatient_ShouldUpdatePatient_WhenPatientExists
├── UpdatePatient_ShouldReturnNotFound_WhenPatientDoesNotExist
├── DeletePatient_ShouldReturnNoContent_WhenPatientExists
└── DeletePatient_ShouldReturnNotFound_WhenPatientDoesNotExist
```

### 🟠 Business Logic

```text
HeartRateServiceTests.cs
└── [Theory] heart rate validation: 40 → true, 100 → true, 200 → true, 250 → false

PatientServiceTests.cs
├── GetPatientFullNameAsync_ShouldReturnFullName_WhenPatientExists
├── GetPatientFullNameAsync_ShouldReturnNull_WhenRepositoryThrowsException
└── GetPatientFullNameAsync_ShouldCallRepositoryOnce
```

### 🌐 Integration Tests

```text
PatientsApiIntegrationTests.cs
├── GetPatient_ReturnsPatient_WhenPatientExists          → 200 OK
├── GetPatient_ReturnsNotFound_WhenPatientDoesNotExist   → 404 Not Found
└── GetPatient_ReturnsOk_WhenValidAdminJwtIsProvided     → 200 OK (Admin JWT)
```

## 📌 Relationship with Day 4

Day 5 doesn't stand alone — it builds directly on Day 4's centralized error handling.

```text
Day 4
Global Exception Middleware
        ↓
Unexpected Exception
        ↓
ILogger
        ↓
ProblemDetails
        ↓
Safe HTTP 500 response

Day 5
Unit Tests + Integration Tests
        ↓
xUnit + Moq + WebApplicationFactory
        ↓
38/38 tests passed
```

Together, Week 5 covered:

```text
Week 5
│
├── Testing
│   ├── Unit Testing
│   ├── Mocking
│   └── Integration Testing
│
└── Error Handling
    ├── Global Middleware
    ├── ProblemDetails
    ├── ILogger
    └── Development / Production behavior
```

## 📌 What I did NOT do

Day 5 explicitly warns against chasing 100% test coverage. I did not try to test every single line of code. Instead, the focus stayed on security, authentication, authorization, business logic, patient CRUD, important API behavior, error scenarios, and integration between components — more professional than simply maximizing the test count.

## 📌 Test project structure

```text
CardiacPatientMonitoring.Tests
│
├── AuthControllerTests.cs
├── JwtServiceTests.cs
├── HeartRateServiceTests.cs
├── PatientServiceTests.cs
├── PatientsControllerTests.cs
├── PatientsApiIntegrationTests.cs
├── CustomWebApplicationFactory.cs
├── TestAuthHandler.cs
└── UnitTest1.cs   (default template, not meaningful)
```

## 📌 Running the tests

```text
Test summary: total: 38, failed: 0, succeeded: 38, skipped: 0
Build succeeded
```

**38 / 38 tests passed ✅**

All hands-on lab deliverables were completed:

1. ✅ Reused the Week 1–4 project skeleton
2. ✅ Identified the 3 highest-risk pieces of logic and wrote unit tests for each
3. ✅ Wrote 3 integration tests covering the Patients API (more than the required 2)
4. ✅ Ran `dotnet test` for the full suite and confirmed everything passes

---

<div align="center">

**Day 5 — Complete ✅**

`Healthcare Management API` · `xUnit` · `Moq` · `WebApplicationFactory` · `38/38 Tests Passing` · `Week 5 Complete`

*— end of Day 5 · end of Week 5*

</div>
