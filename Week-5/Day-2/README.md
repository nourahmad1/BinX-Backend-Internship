<div align="center">

# Day 2 — Mocking Dependencies with Moq

*Field notes from the day I learned how to test service logic without depending on a real database.*

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-Unit%20Testing-5E5E5E?logo=xunit&logoColor=white)
![Moq](https://img.shields.io/badge/Moq-Mocking-6B4FBB?logo=.net&logoColor=white)
![Status](https://img.shields.io/badge/status-complete-2ea44c)

`⏱ 8 hours` · `🧪 29 tests passing` · `🔧 Moq`

</div>

---

## 📌 Today in one sentence

Learning how to isolate a service from its external dependencies using **Moq**, so the tests focus on the service's own business logic instead of a real database or repository.

## 📌 Learning objectives

- Explain why a unit test should isolate its target from real dependencies
- Set up and configure a mock using Moq
- Mock return values and exceptions
- Verify that a mocked dependency was called as expected
- Apply mocking to a real service in the Cardiac Patient Monitoring API

## 📌 Key topics

- Why mock dependencies
- Setting up a mock with Moq
- Mocking return values and exceptions
- Verifying mock interactions

## 📌 What I learned

### 1. Why mock dependencies

A service class that depends on a real database, through a repository interface, can't be unit tested in isolation without either hitting a real database — slow, and not really testing just the service's own logic — or replacing that dependency with a controlled substitute.

A mock replaces the real `IPatientRepository` with an object that returns exactly the data the test specifies, letting the test focus purely on the service's own logic. This only works cleanly because depending on interfaces — not concrete classes — was already the pattern in place:

```csharp
public class PatientService
{
    private readonly IPatientRepository _repository;

    public PatientService(IPatientRepository repository)
    {
        _repository = repository;
    }
}
```

> **Note to self:** depending on interfaces is not just about clean architecture — it also makes the code much easier to test.

### 2. Setting up a mock with Moq

Moq creates a mock implementation of an interface at runtime with `new Mock<T>()`, and `.Setup()` configures what a specific method call on that mock should return. The mock's `.Object` property is the actual fake implementation passed into the service under test, standing in for the real dependency.

```csharp
var mockRepository = new Mock<IPatientRepository>();

mockRepository
    .Setup(repository => repository.GetByIdAsync(1))
    .ReturnsAsync(patient);

var service = new PatientService(mockRepository.Object);
```

The real `PatientService` is being tested, while `IPatientRepository` is controlled entirely by the mock.

### 3. Mocking return values

For the first test, the repository was configured to return a specific patient:

```csharp
var patient = new Patient
{
    Id = 1,
    FirstName = "Ahmad",
    LastName = "Ali"
};

mockRepository
    .Setup(repository => repository.GetByIdAsync(1))
    .ReturnsAsync(patient);
```

Then the real service processes that patient:

```csharp
var result = await service.GetPatientFullNameAsync(1);

Assert.Equal("Ahmad Ali", result);
```

The database is never involved. The test only checks whether `PatientService` correctly transforms the repository result into a full name.

### 4. Mocking exceptions

A real database failure can be difficult to reproduce whenever it needs to be tested. With Moq, that failure can be simulated deliberately using `.ThrowsAsync()`:

```csharp
mockRepository
    .Setup(repository => repository.GetByIdAsync(1))
    .ThrowsAsync(new InvalidOperationException("Database error"));
```

The `PatientService` catches the exception and returns `null`:

```csharp
var result = await service.GetPatientFullNameAsync(1);

Assert.Null(result);
```

This gives a reliable way to test the service's error-handling behavior without actually breaking a database.

### 5. Verifying mock interactions

Checking the returned value is not always enough — the test also needs to confirm the service actually called the repository correctly. Moq's `.Verify()` handles this:

```csharp
mockRepository.Verify(
    repository => repository.GetByIdAsync(1),
    Times.Once);
```

This confirms that the repository method was called, called with patient ID `1`, and called exactly once — useful for catching bugs like a save call being accidentally skipped or duplicated, which checking only the return value would miss.

> **Note to self:** a mock is not only useful for controlling what a dependency returns — it can also prove that the dependency was used correctly. Mocking every single dependency, including simple, cheap value objects with no real behavior to isolate from, adds complexity without adding value. Mock what's slow, external, or genuinely needs to be controlled — not everything a class happens to depend on.

## 📌 What I built — hands-on lab

- [x] Identified `PatientService` as a service that depends on `IPatientRepository`
- [x] Created a `Mock<IPatientRepository>` using Moq
- [x] Configured the mock repository to return a specific patient
- [x] Tested that `PatientService` correctly returns the patient's full name
- [x] Configured the mock repository to throw an exception
- [x] Tested that `PatientService` handles the repository failure and returns `null`
- [x] Used Moq's `Verify()` to confirm the repository method is called exactly once
- [x] Kept the tests isolated from the real database
- [x] Ran the complete test suite and confirmed **29/29 passing**
- [x] Committed the Day 2 testing work to the `feature/week5-cardiac-monitoring` branch

**Tools:** xUnit · Moq · .NET SDK · ASP.NET Core

## 📌 Service under test

The service used for today's hands-on lab is `PatientService`.

```text
PatientService
      │
      │ depends on
      ▼
IPatientRepository
      │
      │ mocked by Moq
      ▼
Mock<IPatientRepository>
```

The repository is responsible for retrieving the patient, while the service is responsible for processing the returned data:

```csharp
public async Task<string?> GetPatientFullNameAsync(int id)
{
    try
    {
        var patient = await _repository.GetByIdAsync(id);

        if (patient is null)
        {
            return null;
        }

        return $"{patient.FirstName} {patient.LastName}";
    }
    catch (Exception)
    {
        return null;
    }
}
```

## 📌 Tests written

**Test 1 — Repository returns a patient**

```csharp
[Fact]
public async Task GetPatientFullNameAsync_ShouldReturnFullName_WhenPatientExists()
{
    // Arrange
    var mockRepository = new Mock<IPatientRepository>();

    var patient = new Patient { Id = 1, FirstName = "Ahmad", LastName = "Ali" };

    mockRepository
        .Setup(repository => repository.GetByIdAsync(1))
        .ReturnsAsync(patient);

    var service = new PatientService(mockRepository.Object);

    // Act
    var result = await service.GetPatientFullNameAsync(1);

    // Assert
    Assert.Equal("Ahmad Ali", result);
}
```

Verifies the service correctly combines the patient's first and last name.

**Test 2 — Repository throws an exception**

```csharp
[Fact]
public async Task GetPatientFullNameAsync_ShouldReturnNull_WhenRepositoryThrowsException()
{
    // Arrange
    var mockRepository = new Mock<IPatientRepository>();

    mockRepository
        .Setup(repository => repository.GetByIdAsync(1))
        .ThrowsAsync(new InvalidOperationException("Database error"));

    var service = new PatientService(mockRepository.Object);

    // Act
    var result = await service.GetPatientFullNameAsync(1);

    // Assert
    Assert.Null(result);
}
```

Verifies the service handles a repository failure instead of letting the exception break the operation.

**Test 3 — Verify repository interaction**

```csharp
mockRepository.Verify(
    repository => repository.GetByIdAsync(1),
    Times.Once);
```

Confirms the repository method is called exactly once with the expected patient ID.

## 📌 Why this is different from Day 1

Day 1 focused on writing tests around controllers and services using xUnit — `[Fact]`, `[Theory]`, Arrange-Act-Assert, JWT testing, and EF Core InMemory.

Day 2 goes one step further by isolating a service from its dependency:

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
```

The main idea is that a unit test should not need a real database just to check whether a service correctly processes a patient.

## 📌 Test project structure

```text
CardiacPatientMonitoring
│
├── CardiacPatientMonitoring.Api
│   ├── Controllers
│   ├── DTOs
│   ├── Entities
│   ├── Services
│   │   ├── HeartRateService.cs
│   │   ├── IJwtService.cs
│   │   ├── JwtService.cs
│   │   ├── IPatientRepository.cs
│   │   └── PatientService.cs
│   └── Data
│
└── CardiacPatientMonitoring.Tests
    ├── AuthControllerTests.cs
    ├── JwtServiceTests.cs
    ├── PatientsControllerTests.cs
    └── PatientServiceTests.cs
```

## 📌 Running the tests

The full test suite was executed after adding the Moq-based service tests:

| Test area | What it covers | Result |
|---|---|---|
| **AuthControllerTests** | Registration, login, authentication and `/me` | ✅ Passing |
| **JwtServiceTests** | JWT generation, expiration and claims | ✅ Passing |
| **PatientsControllerTests** | Patient CRUD and not-found scenarios | ✅ Passing |
| **PatientServiceTests** | Repository mocking, exception handling and service logic | ✅ Passing |

```text
Test summary: total: 29, failed: 0, succeeded: 29, skipped: 0
Build succeeded
```

**29 / 29 tests passed ✅**

## 📌 Git work

The Day 2 work was committed to the feature branch:

```text
feature/week5-cardiac-monitoring
```

Commit:

```text
Add Day 2 Moq unit tests
```

The branch was synchronized with the latest remote changes before pushing the new Day 2 commit.

---

<div align="center">

**Day 2 — Complete ✅**

`Healthcare Management API` · `xUnit` · `Moq` · `Mocked Repository` · `29/29 Tests Passing`

*— end of Day 2*

</div>
