
<div align="center">

# Day 3 — Integration Testing with WebApplicationFactory

*Field notes from the day I stopped testing classes in isolation and started testing the whole API through real HTTP requests.*

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-Integration%20Testing-5E5E5E?logo=xunit&logoColor=white)
![WebApplicationFactory](https://img.shields.io/badge/Mvc.Testing-WebApplicationFactory-512BD4?logo=dotnet&logoColor=white)
![JWT](https://img.shields.io/badge/Auth-JWT-000000?logo=jsonwebtokens&logoColor=white)
![Status](https://img.shields.io/badge/status-complete-2ea44c)

`⏱ 8 hours` · `🌐 Integration Tests` · `🔐 JWT-Protected Endpoints`

</div>

---

## 📌 Today in one sentence

Integration testing the Cardiac Patient Monitoring API using `WebApplicationFactory` and xUnit — testing the application as a whole, through real HTTP requests, the way an actual client experiences it.

## 📌 Learning objectives

- Set up `WebApplicationFactory` to host the API in-memory for testing
- Write integration tests against real HTTP endpoints
- Configure a separate test database isolated from development data
- Test authenticated endpoints using a real test JWT

## 📌 Key topics

- Setting up WebApplicationFactory
- Testing real HTTP endpoints
- Using a test database
- Testing authenticated endpoints

## 📌 What I learned

### 1. WebApplicationFactory hosts the whole application, not just a class

`WebApplicationFactory<Program>` (from `Microsoft.AspNetCore.Mvc.Testing`) spins up the entire API in memory — middleware, routing, dependency injection, controllers, and authentication/authorization — and hands the test an `HttpClient` that talks to it directly, with no real network socket involved.

This is a fundamentally different kind of test than Day 1–2's unit tests: instead of testing individual classes in isolation, it tests the application as a whole, the way a real client actually experiences it.

```csharp
public class PatientsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PatientsApiTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient();
}
```

### 2. Testing real HTTP endpoints catches problems unit tests can't

An integration test sends a real HTTP request and asserts on the actual HTTP response — status code, headers, and deserialized body. This catches issues unit tests miss entirely: incorrect route configuration, middleware misordering, or serialization problems that only show up when a request actually flows through the full pipeline.

For the Patients API, this covered:

- `GET /api/Patients/{id}` when the patient exists → expected `200 OK`, with the complete response body verified (`Id`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `PhoneNumber`, `CreatedAt`)
- `GET /api/Patients/{id}` when the patient does not exist → expected `404 NotFound`

```csharp
[Fact]
public async Task GetPatient_ReturnsNotFound_WhenPatientDoesNotExist()
{
    var response = await _client.GetAsync("/api/Patients/99999");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
}
```

This tests the endpoint through the real HTTP pipeline, instead of calling the controller method directly.

### 3. A separate test database keeps runs isolated and repeatable

Running integration tests against the same database used for manual development risks tests leaving behind data that pollutes future runs — or manual testing.

`CustomWebApplicationFactory` replaces the application's SQL Server `DbContext` registration with an **EF Core InMemory** provider. This keeps test data separate from the development database, avoids modifying real data during tests, and keeps tests isolated and repeatable regardless of what's happening in the regular development database.

### 4. Testing authenticated endpoints with a real test JWT

Testing an `[Authorize]`-protected endpoint requires either generating a real JWT with a test user's claims and attaching it as a bearer token on the test's `HttpClient`, or overriding authentication with a test handler that always succeeds. The former tests the real auth flow end-to-end — which is the approach used here.

The test JWT was built with:

- User ID
- Email
- Full name
- `Admin` role
- Test issuer
- Test audience
- Test signing key

The token is attached to the request as:

```text
Authorization: Bearer <token>
```

Then the test verifies that an authenticated Admin can successfully access:

```text
GET /api/Patients/{id} → 200 OK
```

### 5. Test isolation prevents flaky, order-dependent failures

Integration tests should not share the application's normal database. Using a separate test database prevents test data from affecting other tests, tests from depending on previous test runs, and flaky or order-dependent failures.

> **Note to self:** an integration test suite that shares a single database across runs will eventually produce flaky, order-dependent failures. Resetting or isolating the test database between runs is worth the setup effort before the suite grows much larger.

## 📌 What I built — hands-on lab

- [x] Created `CustomWebApplicationFactory` using `WebApplicationFactory<Program>`
- [x] Created an `HttpClient` to send real HTTP requests to the in-memory application
- [x] Wrote an integration test for the Get-by-id happy path, asserting on the full response body
- [x] Wrote an integration test for the Get-by-id not-found path
- [x] Configured the factory to use a separate EF Core InMemory test database
- [x] Wrote an integration test for a protected endpoint using a valid test JWT with an Admin role
- [x] Verified test isolation from the real development database

**Tools:** Microsoft.AspNetCore.Mvc.Testing · xUnit · Entity Framework Core InMemory · JWT Authentication

## 📌 Why this is different from Day 1 & Day 2

```text
Day 1
Test the application behavior
        ↓
xUnit + real/test database or mocked controller dependencies

Day 2
Test service logic in isolation
        ↓
xUnit + Moq
        ↓
Mock repository dependency

Day 3
Test the application as a whole
        ↓
xUnit + WebApplicationFactory
        ↓
Real HTTP requests through the full middleware pipeline
```

Days 1 and 2 tested individual classes — controllers with mocked dependencies, and services with a mocked repository. Day 3 tests the entire request pipeline end to end, including routing, middleware, DI, and authentication, the way an actual client would hit the API.

## 📌 Test project structure

```text
CardiacPatientMonitoring
│
├── CardiacPatientMonitoring.Api
│   ├── Controllers
│   ├── DTOs
│   ├── Entities
│   ├── Services
│   └── Data
│
└── CardiacPatientMonitoring.Tests
    ├── AuthControllerTests.cs
    ├── JwtServiceTests.cs
    ├── PatientsControllerTests.cs
    ├── PatientServiceTests.cs
    ├── CustomWebApplicationFactory.cs
    └── PatientsApiIntegrationTests.cs
```

## 📌 Running the tests

| Test area | What it covers | Result |
|---|---|---|
| **Get-by-id (happy path)** | `200 OK`, full response body verified | ✅ Passing |
| **Get-by-id (not found)** | `404 NotFound` | ✅ Passing |
| **Test database isolation** | EF Core InMemory replaces SQL Server for tests | ✅ Passing |
| **Authenticated endpoint** | Admin JWT → `200 OK` on protected route | ✅ Passing |

All five hands-on lab tasks were completed and verified:

1. ✅ Set up `WebApplicationFactory`
2. ✅ Test Get-by-id happy path with the full response body
3. ✅ Test Get-by-id not-found path
4. ✅ Configure a separate InMemory test database
5. ✅ Test a protected endpoint using a valid test JWT

---

<div align="center">

**Day 3 — Complete ✅**

`Healthcare Management API` · `xUnit` · `WebApplicationFactory` · `EF Core InMemory` · `JWT Auth Testing`

*— end of Day 3*

</div>
