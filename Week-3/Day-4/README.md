# Task Tracker API — Project Overview

## Overview

Task Tracker API is a RESTful Web API built using **ASP.NET Core (.NET 10)**.

The project manages users and tasks while applying professional backend development practices:

* REST API design
* CRUD operations
* Entity Framework Core
* SQL Server integration
* EF Core migrations
* Async database operations
* DTO pattern
* Validation
* Swagger documentation
* Postman testing

---

# Technologies Used

| Technology                     | Purpose                      |
| ------------------------------ | ---------------------------- |
| C#                             | Backend programming language |
| ASP.NET Core Web API (.NET 10) | REST API development         |
| Entity Framework Core          | ORM and database access      |
| SQL Server LocalDB             | Database                     |
| LINQ                           | Querying                     |
| Swagger/OpenAPI                | Documentation                |
| Postman                        | API testing                  |
| Git & GitHub                   | Version control              |
| draw.io                        | ERD diagrams                 |

---

# Project Structure

```text
TaskTrackerApi

├── Controllers
│   ├── UsersController.cs
│   └── TasksController.cs
│
├── Data
│   └── AppDbContext.cs
│
├── DTOs
│
├── Entities
│
├── Migrations
│
├── Program.cs
└── appsettings.json
```

---

# Architecture

## Entities

Database models:

* User
* TaskItem

## DTOs

Used classes:

* UserDto
* UserCreateDto
* TaskDto
* TaskCreateDto
* TaskUpdateDto

Benefits:

* Prevent exposing database entities
* Control API input
* Add validation

## Controllers

Implemented controllers:

* UsersController
* TasksController

Responsibilities:

* Receive HTTP requests
* Validate input
* Access database
* Return correct responses

---

# Database Design

Entities:

```
Users
Tasks
```

Relationship:

```
Users 1 -------- * Tasks
```

Foreign Key:

```
Tasks.UserId → Users.Id
```

The schema follows:

* 1NF
* 2NF
* 3NF
# Task Tracker API — Entity Framework Core & CRUD

## Entity Framework Core

EF Core is used for:

* Database communication
* LINQ queries
* Entity tracking
* Saving changes
* Migration management

Example:

```csharp
public DbSet<User> Users => Set<User>();

public DbSet<TaskItem> Tasks => Set<TaskItem>();
```

---

# Migrations

Install EF Core CLI:

```bash
dotnet tool install --global dotnet-ef
```

Create migration:

```bash
dotnet ef migrations add InitialCreate
```

Apply migration:

```bash
dotnet ef database update
```

Check migrations:

```bash
dotnet ef migrations list
```

---

# CRUD Endpoints

## Users

| Method | Endpoint          | Description    |
| ------ | ----------------- | -------------- |
| GET    | `/api/users`      | Get all users  |
| GET    | `/api/users/{id}` | Get user by ID |
| POST   | `/api/users`      | Create user    |
| PUT    | `/api/users/{id}` | Update user    |
| DELETE | `/api/users/{id}` | Delete user    |

## Tasks

| Method | Endpoint          | Description    |
| ------ | ----------------- | -------------- |
| GET    | `/api/tasks`      | Get all tasks  |
| GET    | `/api/tasks/{id}` | Get task by ID |
| POST   | `/api/tasks`      | Create task    |
| PUT    | `/api/tasks/{id}` | Update task    |
| DELETE | `/api/tasks/{id}` | Delete task    |

Nested resource:

```
GET /api/users/{id}/tasks
```

---

# Async EF Core

Implemented:

```csharp
ToListAsync()

FirstOrDefaultAsync()

AnyAsync()

SaveChangesAsync()
```

Benefits:

* Non-blocking requests
* Better scalability
* Better performance

---

# Change Tracking

EF Core tracks loaded entities.

Example:

```csharp
var task = await _context.Tasks.FindAsync(id);

task.Title = dto.Title;

await _context.SaveChangesAsync();
```

EF Core detects changes and generates optimized SQL updates.

---

# Validation

Implemented using Data Annotations:

```csharp
[Required]

[MaxLength(200)]

[EmailAddress]
```

Invalid input returns:

```
400 Bad Request
```

---

# HTTP Status Codes

| Code | Meaning            |
| ---- | ------------------ |
| 200  | Successful request |
| 201  | Resource created   |
| 204  | No Content         |
| 400  | Bad Request        |
| 404  | Not Found          |

# Task Tracker API — Testing & Documentation

# Postman Testing

All API endpoints were tested using Postman.

Testing covered:

- Successful requests
- Invalid requests
- Missing resources
- Validation errors
- Correct HTTP status codes

| ID | Method | Endpoint | Expected Result | Screenshot |
|---|---|---|---|---|
| TC-001 | GET | `/api/users` | 200 OK | ![TC-001](images/image.png) |
| TC-002 | GET | `/api/users/1` | 200 OK | ![TC-002](images/image-1.png) |
| TC-003 | GET | `/api/users/99` | 404 Not Found | ![TC-003](images/image-2.png) |
| TC-004 | POST | `/api/users` | 201 Created | ![TC-004](images/image-3.png) |
| TC-005 | POST | `/api/users` Invalid Data | 400 Bad Request | ![TC-005](images/image-4.png) |
| TC-006 | PUT | `/api/users/1` | 204 No Content | ![TC-006](images/image-5.png) |
| TC-007 | PUT | `/api/users/222` | 404 Not Found | ![TC-007](images/image-6.png) |
| TC-008 | DELETE | `/api/users/4` | 204 No Content | ![TC-008](images/image-7.png) |
| TC-009 | DELETE | `/api/users/5555` | 404 Not Found | ![TC-009](images/image-8.png) |
| TC-010 | GET | `/api/tasks` | 200 OK | ![TC-010](images/image-9.png) |
| TC-011 | GET | `/api/tasks/2` | 200 OK | ![TC-011](images/image-10.png) |
| TC-012 | GET | `/api/tasks/99` | 404 Not Found | ![TC-012](images/image-11.png) |
| TC-013 | POST | `/api/tasks` | 201 Created | ![TC-013](images/image-12.png) |
| TC-014 | POST | `/api/tasks` Invalid UserId | 400 Bad Request | ![TC-014](images/image-14.png) |
| TC-015 | PUT | `/api/tasks/2` | 204 No Content | ![TC-015](images/image-15.png) |
| TC-016 | PUT | `/api/tasks/9999` | 404 Not Found | ![TC-016](images/image-16.png) |
| TC-017 | DELETE | `/api/tasks/1` | 204 No Content | ![TC-017](images/image-17.png) |
| TC-018 | DELETE | `/api/tasks/9999` | 404 Not Found | ![TC-018](images/image-18.png) |
         

---

# Swagger Documentation

Swagger was used for:

- Viewing API endpoints
- Sending HTTP requests
- Testing responses
- Checking request and response models


## Swagger Screenshot

![Swagger API Documentation](images/image-19.png)

---

# Tools Used

| Tool                 | Purpose               |
| -------------------- | --------------------- |
| .NET CLI             | Build and run project |
| Entity Framework CLI | Database migrations   |
| SQL Server LocalDB   | Database              |
| Swagger              | API documentation     |
| Postman              | Testing               |
| GitHub               | Source control        |
| draw.io              | Database diagrams     |

---

# Learning Outcomes

After completing this project:

* Built RESTful APIs using ASP.NET Core
* Implemented CRUD operations
* Connected SQL Server with EF Core
* Created migrations
* Used DTO architecture
* Applied async programming
* Implemented validation
* Tested APIs professionally
* Documented backend work

---

# Quality Criteria Achievement

| Category         | Achievement                              |
| ---------------- | ---------------------------------------- |
| API Design       | RESTful routes and correct HTTP codes    |
| Database Quality | Normalized schema and EF migrations      |
| C#/.NET Usage    | Async EF Core and clean patterns         |
| Code Quality     | Organized project structure              |
| Debugging        | Solved configuration and database issues |

---

# Conclusion

Task Tracker API demonstrates professional backend development using ASP.NET Core, Entity Framework Core, SQL Server, REST principles, and API testing tools.


* 2NF
* 3NF
