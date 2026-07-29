# Day 4 - ASP.NET Core Project Setup & Routing

## Overview

In this task, I learned the basics of creating an ASP.NET Core Web API project, configuring the application using the minimal hosting model, creating API endpoints using Controllers and Minimal APIs, and testing them using Postman.

The main goal was to understand how APIs work, how routing is defined, and the difference between Controllers and Minimal APIs.

---

## What I Learned

### 1. Creating ASP.NET Core Web API Project

I learned how to create a new ASP.NET Core Web API project using the .NET CLI:

```bash
dotnet new webapi -o MyFirstApi
```

I also learned how to run the project:

```bash
dotnet run
```

and verify that the API is running successfully.

---

### 2. Understanding the Minimal Hosting Model

I learned how ASP.NET Core applications are configured using the `Program.cs` file.

The main parts are:

- `WebApplication.CreateBuilder(args)`  
  Creates the application builder.

- `builder.Services`  
  Used to register services needed by the application.

- `builder.Build()`  
  Builds the application.

- `app.Run()`  
  Starts the web application.

I also learned how to configure controllers using:

```csharp
builder.Services.AddControllers();
```

and map controller routes using:

```csharp
app.MapControllers();
```

---

## What I Built

### 1. Controller API

I created a `BooksController` to handle book-related requests.

The controller contains:

### Get All Books

Endpoint:

```
GET /api/books
```

This endpoint returns a list of books.

Example response:

```json
[
  "C# Controller",
  "ASP.NET Core Controller",
  "SQL Controller"
]
```

---

### Get Book By ID

Endpoint:

```
GET /api/books/{id}
```

Example:

```
GET /api/books/2
```

This endpoint uses a route parameter to get a specific book.

Example:

```csharp
[HttpGet("{id}")]
public string GetBookById(int id)
{
    return $"Book number {id}";
}
```

---

## 2. Minimal API

I also created the same endpoints using Minimal APIs directly inside `Program.cs`.

### Get All Books

Endpoint:

```
GET /books
```

Implemented using:

```csharp
app.MapGet("/books", () =>
{
    return books;
});
```

---

### Get Book By ID

Endpoint:

```
GET /books/{id}
```

Implemented using:

```csharp
app.MapGet("/books/{id}", (int id) =>
{
    return $"Book number {id}";
});
```

---

## Controller vs Minimal API

During this task, I learned the difference between the two approaches:

### Controllers

- Better for larger applications.
- Organizes related endpoints inside classes.
- Uses attributes like:
  - `[ApiController]`
  - `[Route]`
  - `[HttpGet]`

### Minimal APIs

- Simpler and faster to create.
- Defined directly in `Program.cs`.
- Suitable for small and simple APIs.

---

## API Endpoints Summary

### Controller Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/books` | Get all books |
| GET | `/api/books/{id}` | Get book by ID |

### Minimal API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/books` | Get all books |
| GET | `/books/{id}` | Get book by ID |

---

## Testing

I tested all endpoints using Postman.

The Postman Collection contains:

- Get All Books - Controller
- Get Book By ID - Controller
- Get All Books - Minimal API
- Get Book By ID - Minimal API

---

## Tools Used

- .NET SDK
- ASP.NET Core Web API
- C#
- Postman
- Git & GitHub

---

## Summary

Through this task, I learned how to create and configure an ASP.NET Core Web API project, understand the role of `Program.cs`, create routes using Controllers and Minimal APIs, handle route parameters, and test API endpoints using Postman.

This task provided the foundation for building more advanced APIs in the next stages.
