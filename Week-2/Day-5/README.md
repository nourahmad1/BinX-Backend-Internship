# Day 5 — Middleware Pipeline & Dependency Injection

## Overview

In Day 5, I learned how ASP.NET Core handles incoming requests using the Middleware Pipeline and how Dependency Injection (DI) is used to manage services and dependencies.

The main goal of this task was to create a custom middleware, understand middleware execution order, create a service using an interface, register it using the built-in Dependency Injection container, and inject it into a controller using Constructor Injection.

## What I Learned

During this task, I learned how the ASP.NET Core Middleware Pipeline works and how each middleware component executes in the order it is registered. I also learned how middleware can inspect incoming requests, perform actions, and pass the request to the next component.

I learned why middleware ordering is important and how incorrect ordering can affect application behavior.

I also learned about Dependency Injection and how it helps create loosely coupled and maintainable applications. I practiced working with different service lifetimes:

- Transient: A new instance is created every time the service is requested.
- Scoped: One instance is created per HTTP request.
- Singleton: One instance is created for the entire application lifetime.

## Task Implementation

I created a custom middleware called `RequestLoggingMiddleware` that logs incoming HTTP requests by displaying the HTTP method and request path in the console.

Example:

Request: GET /books
Request: GET /books/1

The middleware was registered inside `Program.cs` using:

```csharp
app.UseMiddleware<RequestLoggingMiddleware>();

I also tested changing the middleware order to understand how the execution order affects the request pipeline.

For Dependency Injection, I created a service layer responsible for handling book data. The service contains:
	•	IBookService interface
	•	BookService implementation

The service was registered using Scoped lifetime:

builder.Services.AddScoped<IBookService, BookService>();

This allows ASP.NET Core to create and provide the required service automatically.

I applied Constructor Injection by injecting IBookService into BooksController instead of creating the service manually.

Example:

public BooksController(IBookService bookService)
{
    _bookService = bookService;
}

This makes the application more flexible, maintainable, and easier to test.

API Endpoints

Get all books:

GET /books

Response:

[
  "C#",
  "ASP.NET Core",
  "SQL"
]

Get book by id:

GET /books/{id}

Example:

GET /books/1

Response:

Book number 1

Project Structure

MiddlewareDependencyInjectionApi

│── Controllers
│   └── BooksController.cs
│
│── Middleware
│   └── RequestLoggingMiddleware.cs
│
│── Services
│   ├── IBookService.cs
│   └── BookService.cs
│
└── Program.cs

Technologies Used
	•	C#
	•	ASP.NET Core Web API
	•	.NET 10
	•	Dependency Injection
	•	Middleware Pipeline
	•	Postman
	•	Git & GitHub

Summary

This task helped me understand how ASP.NET Core processes HTTP requests through middleware and how Dependency Injection improves application structure by separating responsibilities between controllers and services.

I practiced creating custom middleware, registering services with the correct lifetime, applying Constructor Injection, and building a cleaner API structure following ASP.NET Core best practices.
