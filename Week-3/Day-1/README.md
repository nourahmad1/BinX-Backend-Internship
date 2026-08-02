# Task Tracker API

## Project Overview

Task Tracker API is a RESTful Web API built with **ASP.NET Core (.NET 10)**. The project manages users and their tasks while following RESTful API principles learned during the training program.

## Features

- User CRUD operations
- Task CRUD operations
- Nested resource (`/api/v1/users/{id}/tasks`)
- API Versioning (`/api/v1`)
- Proper HTTP status codes
- Swagger API documentation
- Postman API testing
- In-memory data storage using LINQ

---

# Technologies Used

- C#
- ASP.NET Core Web API
- .NET 10
- LINQ
- Swagger (OpenAPI)
- Postman
- Git & GitHub

---

# Project Structure

```text
TaskTrackerApi
│
├── Controllers
│   ├── UsersController.cs
│   └── TasksController.cs
│
├── Models
│   ├── User.cs
│   └── TaskItem.cs
│
├── Data
│   └── AppData.cs
│
├── Program.cs
└── TaskTrackerApi.csproj
```

---

# REST API Endpoints

## Users

| Method | Endpoint | Description | Status |
|---------|----------|-------------|--------|
| GET | `/api/v1/users` | Get all users | 200 OK |
| GET | `/api/v1/users/{id}` | Get user by ID | 200 OK |
| POST | `/api/v1/users` | Create user | 201 Created |
| PUT | `/api/v1/users/{id}` | Update user | 200 OK |
| DELETE | `/api/v1/users/{id}` | Delete user | 204 No Content |

---

## Tasks

| Method | Endpoint | Description | Status |
|---------|----------|-------------|--------|
| GET | `/api/v1/tasks` | Get all tasks | 200 OK |
| GET | `/api/v1/tasks/{id}` | Get task by ID | 200 OK |
| POST | `/api/v1/tasks` | Create task | 201 Created |
| PUT | `/api/v1/tasks/{id}` | Update task | 200 OK |
| DELETE | `/api/v1/tasks/{id}` | Delete task | 204 No Content |

---

## Nested Resource

| Method | Endpoint | Description |
|---------|----------|-------------|
| GET | `/api/v1/users/{id}/tasks` | Get all tasks for a specific user |

---

# HTTP Status Codes

| Code | Meaning | Purpose |
|------|---------|---------|
| 200 | OK | Successful GET or PUT |
| 201 | Created | Resource created successfully |
| 204 | No Content | Resource deleted successfully |
| 400 | Bad Request | Invalid request |
| 404 | Not Found | Resource not found |

---

# LINQ Methods Used

- `Where()`
- `FirstOrDefault()`
- `Any()`

---

# Postman Test Cases

| Test ID | Method | Endpoint | Expected Result | Status | Screenshot |
|---------|--------|----------|-----------------|--------|------------|
| TC-001 | GET | `/api/v1/users` | 200 OK | ✅ PASS | <img src="image-2.png" width="350"/> |
| TC-002 | GET | `/api/v1/users/1` | 200 OK | ✅ PASS | <img src="image-1.png" width="350"/> |
| TC-003 | GET | `/api/v1/users/99` | 404 Not Found | ✅ PASS | <img src="image-3.png" width="350"/> |
| TC-004 | POST | `/api/v1/users` | 201 Created | ✅ PASS | <img src="image-4.png" width="350"/> |
| TC-005 | PUT | `/api/v1/users/1` | 200 OK | ✅ PASS | <img src="image-5.png" width="350"/> |
| TC-006 | DELETE | `/api/v1/users/1` | 204 No Content | ✅ PASS | <img src="image-6.png" width="350"/> |
| TC-007 | GET | `/api/v1/tasks` | 200 OK | ✅ PASS | <img src="image-7.png" width="350"/> |
| TC-008 | GET | `/api/v1/tasks/1` | 200 OK | ✅ PASS | <img src="image-8.png" width="350"/> |
| TC-009 | GET | `/api/v1/tasks/99` | 404 Not Found | ✅ PASS | <img src="image-9.png" width="350"/> |
| TC-010 | POST | `/api/v1/tasks` | 201 Created | ✅ PASS | <img src="image-10.png" width="350"/> |
| TC-011 | POST | `/api/v1/tasks` (Invalid UserId) | 400 Bad Request | ✅ PASS | <img src="image-11.png" width="350"/> |
| TC-012 | PUT | `/api/v1/tasks/1` | 200 OK | ✅ PASS | <img src="image-12.png" width="350"/> |
| TC-013 | DELETE | `/api/v1/tasks/1` | 204 No Content | ✅ PASS | <img src="image-13.png" width="350"/> |
| TC-014 | GET | `/api/v1/users/1/tasks` | 200 OK | ✅ PASS | <img src="image-14.png" width="350"/> |

---

# Screenshots

## Swagger

<p align="center">
<img src="image-15.png" width="850"/>
</p>

---

## Postman Collection

<p align="center">
<img src="image-16.png" width="850"/>
</p>

---

# How to Run

Clone the repository:

```bash
git clone <repository-url>
```

Navigate to the project:

```bash
cd TaskTrackerApi
```

Run the application:

```bash
dotnet run
```

Open Swagger:

```
http://localhost:5138/swagger
```

---

# What I Learned

During this project, I learned how to:

- Build RESTful APIs using ASP.NET Core.
- Design resources following REST conventions.
- Implement CRUD operations.
- Create nested resources.
- Apply API versioning.
- Return appropriate HTTP status codes.
- Use LINQ for querying in-memory data.
- Test APIs with Postman.
- Document APIs using Swagger.
- Organize ASP.NET Core projects using Controllers, Models, and Data folders.
- Document and publish projects professionally on GitHub.
