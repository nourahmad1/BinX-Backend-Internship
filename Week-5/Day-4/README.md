<div align="center">

# Day 4 — Centralized Error Handling & Global Exception Middleware

*Field notes from the day I stopped scattering try/catch everywhere and let one piece of middleware own every unexpected failure.*

[.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
[ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet&logoColor=white)
[Middleware](https://img.shields.io/badge/Middleware-Exception%20Handling-5E5E5E?logo=dotnet)
[ProblemDetails](https://img.shields.io/badge/RFC%207807-ProblemDetails-3178C6)
[ILogger](https://img.shields.io/badge/Logging-ILogger-6B4FBB?logo=dotnet&logoColor=white)
[Status](https://img.shields.io/badge/status-complete-2ea44c)

`⏱ 8 hours` · `🛡️ Global Exception Middleware` · `📋 ProblemDetails`

</div>

---

## 📌 Today in one sentence

Building a centralized error-handling system for the Cardiac Patient Monitoring API — one piece of global middleware that catches unexpected exceptions, logs them properly, and returns a safe, standardized response instead of a `try/catch` scattered across every controller action.

## 📌 Learning objectives

* Explain the problems with scattering `try/catch` throughout every endpoint
* Implement centralized exception-handling middleware
* Return standardized error responses using the `ProblemDetails` format
* Use `ILogger` for structured, queryable logging
* Apply different error behavior for Development and Production environments
* Prevent sensitive exception details from being exposed to clients

## 📌 Key topics

* Scattered vs centralized exception handling
* Global exception-handling middleware
* `ProblemDetails`
* RFC 7807
* Structured logging with `ILogger`
* Development vs Production error handling
* Secure API error responses
* Middleware pipeline

---

## 📌 What I learned

### 1. The problem with scattered try/catch

Wrapping every endpoint in its own `try/catch` creates repeated code and can result in inconsistent error responses across the API.

A centralized middleware solves this problem by sitting in the request pipeline and catching unexpected exceptions from controllers and other downstream components.

Controllers can then focus on expected situations such as:

* `400 Bad Request`
* `401 Unauthorized`
* `403 Forbidden`
* `404 Not Found`

while unexpected runtime exceptions are handled globally.

This makes the API cleaner, easier to maintain, and more consistent.

---

### 2. Global exception-handling middleware

The custom middleware wraps the rest of the request pipeline in a `try/catch`.

When the request is processed normally, `_next(context)` continues the request to the next middleware and eventually to the controller.

If an unexpected exception occurs, the middleware:

1. Catches the exception.
2. Logs the complete exception using `ILogger`.
3. Sets the response status to `500`.
4. Creates a `ProblemDetails` response.
5. Returns a safe response to the client.

The middleware implemented for this project follows this structure:

```csharp
public async Task InvokeAsync(HttpContext context)
{
    try
    {
        await _next(context);
    }
    catch (Exception exception)
    {
        _logger.LogError(
            exception,
            "An unhandled exception occurred while processing {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        await HandleExceptionAsync(
            context,
            exception,
            _environment);
    }
}
```

This means one centralized component is responsible for unexpected exceptions across the entire API.

---

### 3. The ProblemDetails standard

`ProblemDetails` provides a standardized format for HTTP API errors.

The response contains information such as:

* `title` — a general description of the error
* `status` — the HTTP status code
* `detail` — additional information about the error
* `instance` — the request path where the error occurred

For example:

```json
{
  "title": "An unexpected error occurred.",
  "status": 500,
  "detail": "Please try again later.",
  "instance": "/api/Test/error"
}
```

Using a consistent structure makes it easier for API clients to handle errors.

---

### 4. Structured logging with ILogger

The middleware uses `ILogger` to record the complete exception on the server.

Instead of simply logging a message, structured logging allows important information to remain identifiable as separate properties.

For example:

```csharp
_logger.LogError(
    exception,
    "An unhandled exception occurred while processing {Method} {Path}",
    context.Request.Method,
    context.Request.Path);
```

This records useful context such as:

* HTTP method
* Request path
* Exception message
* Exception type
* Stack trace

For example, when the test endpoint fails, the server log contains information about the request:

```text
GET /api/Test/error
```

along with the full exception.

The complete exception is useful for developers and system administrators, but it should not be exposed to external clients in Production.

---

## 📌 Development vs Production error responses

The same deliberate exception was triggered using:

```text
GET /api/Test/error
```

The middleware catches the exception in both environments.

However, the information returned to the client is different.

### 🔹 Development

In Development, the middleware includes the actual exception message in the `detail` field.

**Request:**

```text
GET http://localhost:5052/api/Test/error
```

**Response:**

```json
{
  "title": "An unexpected error occurred.",
  "status": 500,
  "detail": "This is a test exception.",
  "instance": "/api/Test/error"
}
```

The client can see:

```text
This is a test exception.
```

This is useful during development because it helps the developer understand what caused the failure.

The HTTP status is still:

```text
500 Internal Server Error
```

---

### 🔹 Production

In Production, the same exception is caught by the middleware, but the actual exception message is hidden from the client.

**Request:**

```text
GET http://localhost:5000/api/Test/error
```

**Response:**

```json
{
  "title": "An unexpected error occurred.",
  "status": 500,
  "detail": "Please try again later.",
  "instance": "/api/Test/error"
}
```

The client does not receive the original exception message or stack trace.

Instead, it receives:

```text
Please try again later.
```

The complete exception is still logged on the server using `ILogger`.

This allows developers to investigate the problem without exposing internal application information to users.

---

## 🔎 Development vs Production comparison

|                                 | Development                     | Production                          |
| ------------------------------- | ------------------------------- | ----------------------------------- |
| Environment                     | Development                     | Production                          |
| HTTP Status                     | `500`                           | `500`                               |
| Title                           | `An unexpected error occurred.` | `An unexpected error occurred.`     |
| Detail                          | `This is a test exception.`     | `Please try again later.`           |
| Original exception exposed      | ✅ Yes                           | ❌ No                                |
| Stack trace exposed             | ❌ No                            | ❌ No                                |
| Full exception logged on server | ✅ Yes                           | ✅ Yes                               |
| Main purpose                    | Easier debugging                | Security and information protection |

The important point is that **the HTTP status and overall ProblemDetails structure remain consistent**, while the amount of information exposed to the client changes depending on the environment.

---

## 🔐 Why Production should hide exception details

Returning an actual exception message or stack trace in Production can expose sensitive internal information, such as:

* Database errors
* SQL information
* File paths
* Internal class names
* Framework details
* Implementation details
* Stack traces

This information can help an attacker understand how the application works.

Therefore, the API follows this principle:

```text
Development
Exception
   ↓
Middleware catches it
   ↓
ILogger logs full exception
   ↓
Client receives useful exception detail
```

```text
Production
Exception
   ↓
Middleware catches it
   ↓
ILogger logs full exception
   ↓
Client receives safe generic message
```

This gives developers the information they need while protecting the Production API from unnecessary information disclosure.

---

## 📌 What still belongs in the controllers

Global exception handling does not mean controllers should stop handling all errors.

Controllers should still handle **expected errors**.

Examples include:

```text
Patient does not exist
        ↓
404 Not Found
```

```text
Invalid request data
        ↓
400 Bad Request
```

```text
No authentication
        ↓
401 Unauthorized
```

```text
User does not have required role
        ↓
403 Forbidden
```

The global middleware is mainly responsible for **unexpected, unhandled exceptions**, such as:

* Unexpected database failures
* Runtime exceptions
* Null reference errors
* Unexpected service failures
* Other unhandled application errors

Because these exceptions are handled globally, controllers such as:

* `PatientsController`
* `AppointmentsController`
* `AuthController`
* `MedicationsController`
* `VitalSignsController`

do not need a general `try/catch` around every action.

---

## 📌 What I built — Hands-on Lab

* [x] Implemented global exception-handling middleware.
* [x] Returned a standardized `ProblemDetails` response.
* [x] Set the HTTP status code to `500` for unexpected exceptions.
* [x] Added `ILogger` structured logging.
* [x] Logged the HTTP method and request path.
* [x] Logged the complete exception and stack trace on the server.
* [x] Created a test endpoint: `/api/Test/error`.
* [x] Deliberately triggered an unhandled exception.
* [x] Confirmed that the middleware catches the exception.
* [x] Tested the endpoint in Development.
* [x] Confirmed that Development shows the exception message.
* [x] Tested the endpoint in Production.
* [x] Confirmed that Production hides the exception message.
* [x] Confirmed that Production returns `"Please try again later."`.
* [x] Confirmed that the complete exception remains available in server logs.
* [x] Removed redundant general `try/catch` blocks from controllers covered by the global middleware.

---

## 📌 Middleware flow

```text
Request
   │
   ▼
Global Exception Middleware
   │
   ├── _next(context)
   │       │
   │       ▼
   │   Controller
   │       │
   │       └── Normal response
   │
   └── Exception thrown
           │
           ▼
      Catch exception
           │
           ├── ILogger
           │      │
           │      └── Full exception + stack trace
           │
           └── ProblemDetails
                  │
                  ├── Development
                  │      └── Exception message included
                  │
                  └── Production
                         └── Generic safe message
```

---

## 📌 Verifying the middleware

The test endpoint was deliberately designed to throw an exception:

```text
GET /api/Test/error
```

### Development test

The API was running on:

```text
http://localhost:5052
```

The request returned:

```text
500 Internal Server Error
```

with:

```json
{
  "title": "An unexpected error occurred.",
  "status": 500,
  "detail": "This is a test exception.",
  "instance": "/api/Test/error"
}
```

The Development response confirmed that the middleware successfully caught the exception and returned `ProblemDetails`.

---

### Production test

The API was running on:

```text
http://localhost:5000
```

The request returned:

```text
500 Internal Server Error
```

with:

```json
{
  "title": "An unexpected error occurred.",
  "status": 500,
  "detail": "Please try again later.",
  "instance": "/api/Test/error"
}
```

The Production response confirmed that the exception details were hidden from the client.

The full exception was still recorded in the server log.

> **Important:** Swagger is configured for Development in this project. Therefore, the Development test can be performed through Swagger, while the Production endpoint can be tested directly using the API URL or another HTTP client such as Postman.

---

## 📌 Why this is different from Days 1–3

### Day 1 — Controller/API testing

```text
Test API behavior
       ↓
xUnit
       ↓
Controller/API tests
```

### Day 2 — Unit testing

```text
Test service logic in isolation
       ↓
xUnit + Moq
       ↓
Mock dependencies
```

### Day 3 — Integration testing

```text
Test the application as a whole
       ↓
xUnit + WebApplicationFactory
       ↓
Real HTTP requests
       ↓
Full application pipeline
```

### Day 4 — Global error handling

```text
Unexpected application failure
       ↓
Global Exception Middleware
       ↓
ILogger
       ↓
ProblemDetails
       ↓
Safe and consistent API response
```

Days 1–3 focused mainly on **testing and proving that the API behaves correctly**.

Day 4 focuses on **what happens when something unexpected goes wrong**.

Instead of every controller handling unexpected exceptions separately, the middleware provides one centralized solution for the entire API.

---

## 📌 Project structure

```text
CardiacPatientMonitoring
│
├── CardiacPatientMonitoring.Api
│   ├── Controllers
│   │   ├── PatientsController.cs
│   │   ├── AppointmentsController.cs
│   │   ├── AuthController.cs
│   │   ├── MedicationsController.cs
│   │   ├── VitalSignsController.cs
│   │   └── TestController.cs
│   │
│   ├── Middleware
│   │   └── ExceptionHandlingMiddleware.cs
│   │
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

---

## 📌 Final result

All five hands-on lab requirements were completed and verified:

1. ✅ Implemented global exception-handling middleware returning `ProblemDetails`.
2. ✅ Confirmed that Production does not expose the actual exception message or stack trace.
3. ✅ Added structured logging with `ILogger`, including request method and request path.
4. ✅ Deliberately triggered an unhandled exception through `/api/Test/error` and confirmed that the middleware catches it.
5. ✅ Removed redundant general `try/catch` blocks from individual endpoints covered by the global handler.

The final result is a more consistent, maintainable, and secure API error-handling system.

---

<div align="center">

**Day 4 — Complete ✅**

`Healthcare Management API` · `Global Exception Middleware` · `ProblemDetails` · `ILogger` · `Development/Production Error Handling`

*— end of Day 4*

</div>
