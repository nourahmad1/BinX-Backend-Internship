<div align="center">

# Day 1 — Choosing Your Phase 3 Project & Unit Testing with xUnit

*Field notes from the day I started turning the Cardiac Patient Monitoring API into a project that can be tested, not just built.*

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-Unit%20Testing-5E5E5E?logo=xunit&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?logo=dotnet&logoColor=white)
![Status](https://img.shields.io/badge/status-complete-2ea44c)

`⏱ 8 hours` · `🧪 26 tests passing` · `📌 Phase 3 Capstone`

</div>

---

## 📌 Today in one sentence

Choosing a realistic Phase 3 capstone project and learning how to start testing a real ASP.NET Core API with **xUnit** — writing unit tests that verify important application behavior instead of just trusting that it works.

## 📌 Learning objectives

- Choose and scope a Phase 3 capstone project
- Understand `[Fact]` vs `[Theory]` in xUnit
- Apply the Arrange-Act-Assert pattern
- Write controller and service tests using Moq and EF Core InMemory

## 📌 Project scope

> The Cardiac Patient Monitoring API will provide a REST API for managing patients and their cardiac monitoring data, including authentication, patient records, vital signs, medications, and appointments. The project will use ASP.NET Core, Entity Framework Core, SQL Server, JWT authentication, role-based access control, validation, and automated testing. By Week 9, the project will include API documentation, a Postman collection, database migrations and ERD, security, unit and integration tests, deployment, CI/CD, and a complete README.


## 📌 What I learned

### 1. Picking a project is really about picking a scope
Out of six available directions — Task & Project Management API, E-Commerce Backend, Booking & Reservation System API, Social Platform Backend, Healthcare Management API, CRM Backend — I went with **Healthcare Management API** and continued the project already in progress: the **Cardiac Patient Monitoring API**. It manages cardiac patients and their monitoring data through a RESTful backend covering registration, JWT auth, patients, vital signs, medications, and appointments (full scope above).

### 2. `[Fact]` and `[Theory]` cover different jobs
A `[Fact]` checks one specific scenario:

```csharp
[Fact]
public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
{
    // Arrange
    // Prepare the test data and dependencies.

    // Act
    // Call the method being tested.

    // Assert
    // Verify the result.
}
```

A `[Theory]` runs the same test against multiple inputs instead of copy-pasting near-identical methods:

```csharp
[Theory]
[InlineData(10, 5, 15)]
[InlineData(-3, 3, 0)]
[InlineData(0, 10, 10)]
public void Add_ReturnsCorrectSum(int a, int b, int expected)
{
    // Arrange
    // Act
    // Assert
}
```

### 3. Arrange-Act-Assert keeps tests readable
Separating a test into **Arrange** (test data, the object under test, mocks, expected behavior), **Act** (call the method), and **Assert** (verify the result) makes each test easier to follow — and much easier to debug when something fails.

### 4. Mocking keeps tests focused on the controller, not its dependencies
Testing `AuthController` login didn't require a real `UserManager` or JWT service — Moq stands in for both:

```csharp
userManager
    .Setup(x => x.FindByEmailAsync("test@example.com"))
    .ReturnsAsync(user);

userManager
    .Setup(x => x.CheckPasswordAsync(user, "Password123!"))
    .ReturnsAsync(true);

_jwtServiceMock
    .Setup(x => x.GenerateToken(user))
    .Returns(new AuthTokenResult
    {
        Token = "fake-jwt-token",
        ExpiresAt = DateTime.UtcNow.AddMinutes(60)
    });
```

For invalid login, the test also checks that a token is **never** generated — verifying behavior, not just the response:

```csharp
_jwtServiceMock.Verify(
    x => x.GenerateToken(It.IsAny<ApplicationUser>()),
    Times.Never);
```

### 5. Testing a JWT means reading it back, not just checking it exists
`JwtService` tests decode the generated token and assert on its actual claims — user ID, email, full name, and a future expiration — instead of only checking that a non-empty string came back:

```csharp
var handler = new JwtSecurityTokenHandler();
var token = handler.ReadJwtToken(result.Token);

Assert.Equal(
    user.Id,
    token.Claims.First(
        c => c.Type == JwtRegisteredClaimNames.Sub).Value);
```

### 6. EF Core InMemory isolates database tests from the real database
`PatientsController` depends on `AppDbContext`, so its tests run against an **EF Core InMemory** database instead of the real SQL Server instance — covering get all, get by ID, create, update, delete, and the not-found case, without touching development data.

> **Note to self:** testing isn't something to bolt on at the end — writing the test alongside the feature is what actually catches bad assumptions early.

## 📌 What I built — hands-on lab

- [x] Selected the **Healthcare Management API** project direction and scoped the Cardiac Patient Monitoring API
- [x] Created a separate `CardiacPatientMonitoring.Tests` xUnit project referencing the API project
- [x] Learned `[Fact]` and `[Theory]`, and applied the Arrange-Act-Assert pattern
- [x] Wrote `AuthControllerTests` covering register, login, and `/me` (success and failure paths)
- [x] Wrote `JwtServiceTests` verifying token generation and claim contents
- [x] Wrote `PatientsControllerTests` using EF Core InMemory for CRUD and not-found scenarios
- [x] Used Moq to isolate `UserManager` and the JWT service from the controller under test
- [x] Ran the full test suite and confirmed **26/26 passing**

**Tools:** xUnit · Moq · Entity Framework Core InMemory

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
    └── PatientsControllerTests.cs
```

## 📌 Running the tests

With the full suite in place, `dotnet test` was run against the project:

| Test class | What it covers | Result |
|---|---|---|
| **AuthControllerTests** | Register (success, existing email), Login (valid, invalid), Me (found, not found) | ✅ Passing |
| **JwtServiceTests** | Token generated, not empty, future expiration, correct claims (ID, email, name) | ✅ Passing |
| **PatientsControllerTests** | Get all, get by ID, create, update, delete, not-found handling (EF Core InMemory) | ✅ Passing |

```text
Test summary: total: 26, failed: 0, succeeded: 26, skipped: 0
Build succeeded
```

**26 / 26 tests passed ✅**

---

<div align="center">

**Day 1 — Complete ✅**

`Healthcare Management API` · `xUnit` · `Moq` · `Arrange-Act-Assert` · `26/26 Tests Passing`

*— end of Day 1*

</div>
