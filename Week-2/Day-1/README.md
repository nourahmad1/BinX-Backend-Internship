# Generic Repository Lab

## Overview

In this task, I learned how Generics work in C# and why they are useful when building reusable and type-safe code.

The main goal was to create a generic repository that can work with different types of objects without repeating the same code for each type.

---

# What I Learned

## 1. Why Generics?

Before Generics, collections usually stored data as `object`.

This caused some problems:

- We needed to use casting when reading values.
- The compiler could not always detect type mistakes.
- Errors could appear during runtime.

Generics solve this problem by allowing us to define the type when using the class or collection.

Example:

```csharp
List<int>
```

can store only integers.

```csharp
List<string>
```

can store only strings.

This gives us:

- Type safety
- Less casting
- Errors detected earlier during compile time

---

## 2. Generic Classes

I learned how to create a class that can work with different data types using a type parameter.

Example:

```csharp
Repository<T>
```

The `T` represents the type that will be used later.

For example:

```csharp
Repository<Book>
```

creates a repository for books.

```csharp
Repository<Member>
```

creates a repository for members.

The same class can be reused with different types.

---

## 3. Generic Constraints

I learned how to restrict the types that can be used with Generics.

In this task, I used:

```csharp
where T : class
```

This means that the repository only accepts reference types such as:

- Book
- Member
- Student

and does not accept value types like:

- int
- double
- bool

This makes the Generic class safer and matches the purpose of a repository.

---

## 4. Collection Interfaces

I learned the difference between collection interfaces:

### IEnumerable<T>

Used when we only need to iterate through items.

Example:

```csharp
foreach()
```

---

### IReadOnlyList<T>

Used when we want to allow reading data but prevent modification.

It allows:

- Reading items
- Accessing items by index
- Getting the count

But it does not allow:

- Add
- Remove
- Clear

---

### IList<T>

Allows full modification of the collection.

It can add, remove, and update items.

---

# Task Description

The task was to build a Generic Repository.

The repository should be reusable and able to work with different object types.

The requirements were:

1. Create a generic `Repository<T>` class.
2. Add a `where T : class` constraint.
3. Add methods:
   - `Add()`
   - `GetAll()`
   - `Find()`
4. Use the repository with two different types from the domain model.
5. Change `GetAll()` return type to `IReadOnlyList<T>` to prevent direct modification.
6. Commit the work to GitHub.

---

# Implementation

I created a generic repository:

```csharp
public class Repository<T> where T : class
```

Inside the repository, I used:

```csharp
private readonly List<T> _items = new();
```

to store the objects.

---

## Repository Methods

### Add()

Used to add new items to the repository.

Example:

```csharp
bookRepository.Add(new Book("C#", "Microsoft"));
```

---

### GetAll()

Returns all stored items as:

```csharp
IReadOnlyList<T>
```

This allows users to view the data without changing the original collection.

---

### Find()

Used to search for a specific item using a condition.

Example:

```csharp
Find(book => book.Title == "C#")
```

---

# Testing the Repository

I tested the repository with two different classes:

## Book Repository

```csharp
Repository<Book>
```

It stores and manages books.

---

## Member Repository

```csharp
Repository<Member>
```

It stores and manages members.

---

# What I Practiced

During this task, I practiced:

- Creating Generic classes
- Using Type Parameters
- Applying Generic Constraints
- Working with Collection Interfaces
- Understanding IReadOnlyList and why it is safer
- Writing reusable code
- Using Git and GitHub workflow

---

# Conclusion

This task helped me understand how Generics improve code reusability and safety in C#.

Instead of creating separate classes for every object type, I learned how to create one flexible repository that can work with different types while keeping the code clean and maintainable.
