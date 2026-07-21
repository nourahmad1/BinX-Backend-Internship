# Day 3 - C# Fundamentals II: Object-Oriented Programming

## What I Learned Today

Today I learned the main concepts of Object-Oriented Programming (OOP) in C# and how they are used to build clean and organized applications.

The main topics I learned were:

* Classes, Records, and Structs
* Encapsulation and Access Modifiers
* Inheritance
* Interfaces
* Polymorphism

---

# 1. Classes, Records, and Structs

## Classes

A class is used when we need to create an object that has its own identity and behavior.

For example:

* User
* Product
* Order
* Book

A class can contain:

* Properties
* Fields
* Methods
* Constructors

Classes are usually used for objects that represent real things in a system.

---

## Records

A record is used mainly for storing data.

Records are useful for:

* API requests
* API responses
* Data Transfer Objects (DTOs)

The main difference from classes is that records compare objects based on their values, not their identity.

Example:

Two records with the same data are considered equal.

---

## Structs

A struct is a value type used for small data objects.

Examples:

* Coordinates
* Points
* Money values

Structs are useful when we need a small object that is copied by value.

---

# 2. Encapsulation

Encapsulation means protecting the internal data of a class and controlling how it can be accessed.

Instead of allowing anyone to directly change data, we use:

* Private fields
* Public properties
* Validation inside constructors or methods

Example:

```csharp
private string email;
```

The data cannot be changed directly from outside the class.

A property allows controlled access:

```csharp
public string Email
{
    get
    {
        return email;
    }
}
```

This makes the code safer and easier to maintain.

---

# 3. Access Modifiers

I learned that access modifiers control who can access class members.

The main modifiers are:

## public

Accessible from anywhere.

Example:

```csharp
public string Name;
```

---

## private

Accessible only inside the same class.

Example:

```csharp
private int age;
```

Used to protect data.

---

## protected

Accessible inside the class and classes that inherit from it.

---

## internal

Accessible inside the same project.

---

# 4. Inheritance

Inheritance allows one class to reuse another class's properties and methods.

It represents an "is-a" relationship.

Example:

```
Animal
   |
   |
  Dog
```

A Dog is an Animal, so it can inherit from Animal.

Benefits:

* Reduce duplicated code
* Reuse existing behavior
* Make code easier to organize

---

# 5. Interfaces

An interface defines a contract that classes must follow.

It tells a class what it should do, but not how it should do it.

Example:

```csharp
interface IBorrowable
{
    void Borrow();
}
```

Any class that implements this interface must contain:

```csharp
Borrow()
```

Interfaces are useful when different classes have the same ability but are not related by inheritance.

Example:

* Book can be borrowed
* Member can borrow

They are different objects, but they share the same behavior.

---

# 6. Polymorphism

Polymorphism means that the same code can work with different types.

Instead of creating separate methods for every type, we can use an interface.

Example:

Instead of:

```csharp
Borrow(Book book)

Borrow(Member member)
```

We use:

```csharp
Borrow(IBorrowable item)
```

Now any object that implements `IBorrowable` can be used.

The program automatically calls the correct implementation.

This makes the code:

* More flexible
* Easier to extend
* Easier to maintain

---

# Task Implementation

For the practical task, I created a simple Library System.

The system contains:

## Book Class

Represents a book in the library.

It contains:

* Title
* Author

It uses encapsulation by keeping fields private and exposing properties.

---

## Member Class

Represents a library member.

It contains:

* Name
* Email

It also applies encapsulation and validation.

---

## IBorrowable Interface

I created an interface:

```csharp
IBorrowable
```

It contains:

```csharp
void Borrow();
```

Both Book and Member implement this interface.

---

## BorrowRequest Record

I created a record:

```csharp
BorrowRequest
```

It is used as a Data Transfer Object to store borrowing request information.

It contains:

* Member name
* Book title

---

# What I Practiced in This Task

During this task, I practiced:

Creating classes in C#
* Using constructors
* Applying validation
* Using private fields and public properties
* Creating and implementing interfaces
* Understanding polymorphism
* Choosing between class and record
* Organizing a small domain model

---

# Final Understanding

The main idea I learned today is that OOP helps us organize code by creating objects that represent real-world things.

* Classes represent objects with identity and behavior.
* Records represent data.
* Structs represent small values.
* Encapsulation protects data.
* Inheritance allows code reuse.
* Interfaces define shared behavior.
* Polymorphism allows the same code to work with different objects.

This knowledge will help me build cleaner applications using C# and .NET.
