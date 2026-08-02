# Day 5 — Middleware Pipeline & Dependency Injection

## Overview

Day 5 focuses on understanding two important concepts in ASP.NET Core: the Middleware Pipeline and Dependency Injection.

The purpose of this day is to understand how ASP.NET Core processes incoming HTTP requests, how middleware components work together, and how services are managed and provided throughout the application using the built-in Dependency Injection system.

## What I Learned

During this day, I learned:

- How the ASP.NET Core Middleware Pipeline works and how requests move through a sequence of middleware components.
- How the order of middleware registration affects request execution and application behavior.
- The role of built-in middleware such as HTTPS Redirection, Routing, Authentication, and Authorization.
- How to create and use custom middleware for handling common application concerns like logging and request processing.
- What Dependency Injection is and why it is used in modern application development.
- How to register services inside the ASP.NET Core DI container.
- The differences between service lifetimes:
  - Transient
  - Scoped
  - Singleton
- How Constructor Injection allows controllers and services to receive their required dependencies automatically.
- Why using interfaces and dependency injection improves code flexibility, maintainability, and testability.

## Day 5 Topics

The main topics covered in this day were:

### Middleware Pipeline

Understanding how every HTTP request passes through a chain of middleware components and how each component can process the request before and after passing it to the next component.

### Middleware Ordering

Learning that middleware execution order is important because each middleware depends on its position in the request pipeline.

### Dependency Injection

Understanding how ASP.NET Core manages object creation and provides required services automatically instead of creating dependencies manually.

### Service Lifetimes

Learning when to use each service lifetime depending on how long the service instance should live:

- Transient for short-lived services.
- Scoped for services that should exist during a single request.
- Singleton for services shared throughout the whole application.

### Constructor Injection

Learning how controllers receive required services through constructors and how this approach helps achieve cleaner and more maintainable code.

## Hands-On Practice

The practical task for this day focused on applying Middleware and Dependency Injection concepts by creating a custom middleware, working with services, registering dependencies, and using injected services inside controllers.

## Technologies Used

- C#
- ASP.NET Core Web API
- .NET
- Dependency Injection
- Middleware Pipeline
- Git & GitHub
- Postman

## Summary

Day 5 provided a deeper understanding of ASP.NET Core application structure by learning how requests are handled through middleware and how dependencies are managed using Dependency Injection.

These concepts are essential for building scalable, organized, and maintainable backend applications.
