# Library System - Day 4

## About This Project

This project is a continuation of the Library System built in previous days.

In this version, I practiced working with collections, writing LINQ queries, creating asynchronous methods, and handling exceptions in C#.

---

## What I Learned

Today I learned how to choose the right collection depending on the situation.

- List<T> is useful for storing ordered data.
- Dictionary<TKey, TValue> is better when searching by a key.
- HashSet<T> is useful when duplicate values are not allowed.

I also learned the basics of LINQ. Instead of writing long loops, I can query data using methods like:

- Where()
- Select()
- Count()

This makes the code shorter and easier to read.

Another important topic was async and await. I learned that asynchronous methods allow the program to wait for long operations, such as reading files or calling a database, without freezing the application.

Finally, I learned how to handle errors using try and catch. Instead of letting the program crash, I can catch specific exceptions like FormatException and display a helpful message to the user.

---

## What This Program Does

The program creates a library with eight books.

It then:

- Displays all books.
- Shows only the borrowed books using LINQ.
- Displays the titles of all books.
- Counts how many books are currently borrowed.
- Simulates loading library data using an asynchronous method.
- Asks the user to enter a library card number and safely handles invalid input using exception handling.

---

## Concepts Used

- Classes
- Objects
- Constructors
- Methods
- List<T>
- LINQ
- Where()
- Select()
- Count()
- async / await
- Task.Delay()
- try / catch
- FormatException

---

## Result

This project helped me understand how collections work, how LINQ can simplify data processing, how asynchronous programming improves responsiveness, and how exception handling makes applications more reliable.
