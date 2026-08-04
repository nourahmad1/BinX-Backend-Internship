# Task Tracker API

## Overview

Task Tracker API is a RESTful Web API built using **ASP.NET Core (.NET 10)** and **Entity Framework Core**.

The purpose of this project is to practice building a backend API with a clean structure, database integration, migrations, and CRUD operations.

The API manages users and their tasks using a relational database with a one-to-many relationship:

* One User can have many Tasks.
* Each Task belongs to one User.

---

# Technologies Used

* ASP.NET Core Web API (.NET 10)
* C#
* Entity Framework Core 10
* SQL Server LocalDB
* LINQ
* Swagger / OpenAPI
* Entity Framework Core Migrations
* Git & GitHub

---

# Project Structure

```
TaskTrackerApi
│
├── Controllers
│   ├── UsersController.cs
│   └── TasksController.cs
│
├── Data
│   └── AppDbContext.cs
│
├── DTOs
│   ├── UserDto.cs
│   ├── UserCreateDto.cs
│   ├── TaskDto.cs
│   ├── TaskCreateDto.cs
│   └── TaskUpdateDto.cs
│
├── Entities
│   ├── User.cs
│   └── TaskItem.cs
│
├── Migrations
│
├── Program.cs
└── appsettings.json
```

---

# Features Implemented

## User Management

The API supports:

* Get all users
* Get user by ID
* Create new users
* Delete users

---

## Task Management

The API supports:

* Get all tasks
* Get task by ID
* Create tasks
* Update tasks
* Delete tasks

---

# Database Design

The project uses Entity Framework Core Code First approach.

Database relationship:

```
User
 |
 | 1 : Many
 |
TaskItem
```

Example:

```
Users Table

Id
Name
Email


Tasks Table

Id
Title
IsCompleted
UserId
```

`UserId` is a foreign key connecting tasks with users.

---

# Entity Framework Core Setup

The project uses:

```csharp
public DbSet<User> Users => Set<User>();

public DbSet<TaskItem> Tasks => Set<TaskItem>();
```

These properties allow Entity Framework Core to track and manage database tables.

---

# Migrations

The database was created using EF Core migrations.

Commands used:

Create migration:

```bash
dotnet ef migrations add InitialCreate
```

Apply migration:

```bash
dotnet ef database update
```

Create validation changes:

```bash
dotnet ef migrations add AddUserValidation
```

---

# API Endpoints

## Users

### Get All Users

```
GET /api/users
```

---

### Get User By Id

```
GET /api/users/{id}
```

---

### Create User

```
POST /api/users
```

Request:

```json
{
  "name": "Nour",
  "email": "nour@test.com"
}
```

---

### Delete User

```
DELETE /api/users/{id}
```

---

# Tasks

### Get All Tasks

```
GET /api/tasks
```

---

### Get Task By Id

```
GET /api/tasks/{id}
```

---

### Create Task

```
POST /api/tasks
```

Request:

```json
{
  "title": "Learn Entity Framework Core",
  "userId": 1
}
```

---

### Update Task

```
PUT /api/tasks/{id}
```

Request:

```json
{
  "title": "Finish API",
  "isCompleted": true
}
```

---

### Delete Task

```
DELETE /api/tasks/{id}
```

---

# Validation

DTO validation was implemented using Data Annotations.

Examples:

```csharp
[Required]
[MaxLength(100)]
public string Name { get; set; }
```

Validation prevents invalid data from being stored in the database.

---

# DTO Usage

The project uses Data Transfer Objects instead of exposing database entities directly.

Benefits:

* Better API security
* Separation between database and API models
* Easier future changes
* Cleaner responses

Example:

```
Client
  |
  |
DTO
  |
  |
Controller
  |
  |
Entity
  |
  |
Database
```

---

# Running the Project

Clone the repository:

```bash
git clone <repository-url>
```

Navigate to project folder:

```bash
cd TaskTrackerApi
```

Restore dependencies:

```bash
dotnet restore
```

Apply database migrations:

```bash
dotnet ef database update
```

Run the API:

```bash
dotnet run
```

---

# Swagger Documentation

Swagger is enabled for testing and documenting API endpoints.

Open:

```
http://localhost:5006/swagger
```

Swagger allows testing:

* GET requests
* POST requests
* PUT requests
* DELETE requests

without using external tools.

---

# Learning Outcomes

Through this task, I practiced:

* Creating ASP.NET Core Web APIs
* Working with Controllers
* Dependency Injection
* Entity Framework Core
* Database migrations
* SQL Server integration
* DTO design pattern
* Validation
* REST API principles

---

# Future Improvements

Possible future enhancements:

* Add authentication and authorization
* Add pagination
* Add global exception handling
* Add repository/service layers
* Add automated testing
* Add logging
