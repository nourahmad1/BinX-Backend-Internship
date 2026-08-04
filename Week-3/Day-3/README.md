# Task Tracker API

## Week 3 - Day 3 Backend Development Task

## Project Overview

Task Tracker API is a RESTful Web API developed using **ASP.NET Core (.NET 10)**.

The goal of this project is to practice building a real backend application by connecting an ASP.NET Core API with a SQL Server database using **Entity Framework Core**.

The application manages users and their tasks through CRUD operations while following REST API principles and clean backend development practices.

The project demonstrates:

* Creating Web API controllers
* Working with Entity Framework Core
* Database-first communication using migrations
* SQL Server integration
* Entity relationships
* DTO pattern implementation
* Input validation
* API testing using Swagger

---

# Development Environment

## Framework

* .NET 10
* ASP.NET Core Web API

## Programming Language

* C#

## Database

* SQL Server LocalDB

## IDE / Editor

* Visual Studio Code

## Operating System

* Windows

---

# Tools Used

## .NET CLI

Used for creating, building, running, and managing the project.

Commands used:

```bash
dotnet --version
```

Check installed .NET SDK version.

```bash
dotnet build
```

Compile the project and verify that there are no build errors.

```bash
dotnet run
```

Run the ASP.NET Core Web API.

```bash
dotnet restore
```

Restore project dependencies.

---

## Entity Framework Core Tools

Used for database management and migrations.

Installation:

```bash
dotnet tool install --global dotnet-ef
```

Check EF Core tools:

```bash
dotnet ef --version
```

Used commands:

Create migration:

```bash
dotnet ef migrations add InitialCreate
```

Apply migration:

```bash
dotnet ef database update
```

List migrations:

```bash
dotnet ef migrations list
```

---

## Swagger / OpenAPI

Swagger was used to document and test API endpoints.

Benefits:

* View available endpoints
* Send HTTP requests
* Test API responses
* Validate request models

Swagger URL:

```
http://localhost:5006/swagger
```

---

## SQL Server LocalDB

Used as the relational database.

The database stores:

* Users
* Tasks
* Relationships between entities

Database created:

```
TaskTrackerDb
```

---

## Git & GitHub

Used for:

* Source code management
* Tracking changes
* Version control
* Submitting project progress

Common commands:

```bash
git status
```

Check project changes.

```bash
git add .
```

Stage changes.

```bash
git commit -m "message"
```

Save changes.

```bash
git push
```

Upload changes to GitHub.

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

# Architecture Explanation

The project follows a simple layered structure.

## Entities Layer

Contains database models.

Example:

```
User
TaskItem
```

These classes represent database tables.

---

## DTO Layer

DTOs are used to control data sent between client and API.

Examples:

```
UserCreateDto
TaskCreateDto
TaskUpdateDto
```

Benefits:

* Prevent exposing database entities
* Control incoming data
* Add validation rules
* Improve API security

---

## Controller Layer

Controllers handle HTTP requests.

Responsibilities:

* Receive requests
* Validate input
* Communicate with database
* Return responses

Controllers:

```
UsersController
TasksController
```

---

## Data Layer

Contains:

```
AppDbContext
```

It manages communication between Entity Framework Core and SQL Server.

---

# Database Design

The project uses a One-to-Many relationship.

Relationship:

```
User
 |
 |
 |  One User
 |
 |
Many Tasks
```

Example:

## Users Table

| Column | Type     |
| ------ | -------- |
| Id     | int      |
| Name   | nvarchar |
| Email  | nvarchar |

---

## Tasks Table

| Column      | Type     |
| ----------- | -------- |
| Id          | int      |
| Title       | nvarchar |
| IsCompleted | bit      |
| UserId      | int      |

`UserId` works as a foreign key.

---

# Entity Framework Core Implementation

The database context contains:

```csharp
public DbSet<User> Users => Set<User>();

public DbSet<TaskItem> Tasks => Set<TaskItem>();
```

These properties allow EF Core to:

* Track entities
* Create tables
* Execute queries
* Save changes

---

# Implemented Features

## User Features

### Get All Users

```
GET /api/users
```

Returns all users.

---

### Get User By ID

```
GET /api/users/{id}
```

Returns specific user.

---

### Create User

```
POST /api/users
```

Example request:

```json
{
"name":"Nour",
"email":"nour@test.com"
}
```

---

### Delete User

```
DELETE /api/users/{id}
```

---

# Task Features

## Get All Tasks

```
GET /api/tasks
```

## Get Task By ID

```
GET /api/tasks/{id}
```

## Create Task

```
POST /api/tasks
```

Example:

```json
{
"title":"Learn EF Core",
"userId":1
}
```

## Update Task

```
PUT /api/tasks/{id}
```

Example:

```json
{
"title":"Complete API",
"isCompleted":true
}
```

## Delete Task

```
DELETE /api/tasks/{id}
```

---

# Validation

Validation was implemented using Data Annotations.

Example:

```csharp
[Required]
[MaxLength(100)]
public string Name {get;set;}
```

Implemented validations:

* Required fields
* Maximum length
* Email format validation

---

# Problems Solved During Development

## Entity Framework Command Not Found

Problem:

```
dotnet ef command not found
```

Solution:

Installed EF Core CLI:

```
dotnet tool install --global dotnet-ef
```

---

## SQL Server Connection Problem

Problem:

LocalDB instance was not running.

Solution:

Started SQL LocalDB:

```
sqllocaldb start MSSQLLocalDB
```

Then applied migrations:

```
dotnet ef database update
```

---

## Swagger Build Issue

Problem:

Missing Swagger package methods.

Solution:

Added Swagger dependency and configured:

```csharp
builder.Services.AddSwaggerGen();

app.UseSwagger();

app.UseSwaggerUI();
```

---

# Learning Outcomes

After completing this task, I learned:

* How to create ASP.NET Core Web APIs
* How dependency injection works
* How controllers handle HTTP requests
* How EF Core communicates with databases
* How migrations manage database changes
* How DTOs improve API design
* How to build clean CRUD operations
* How to test APIs using Swagger
* How to manage project changes using Git

---


# Conclusion

Task Tracker API provided practical experience in backend API development using modern .NET technologies.

The project combines ASP.NET Core, Entity Framework Core, SQL Server, and REST principles to create a structured and maintainable backend application.
