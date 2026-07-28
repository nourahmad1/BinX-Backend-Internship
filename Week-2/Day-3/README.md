# Async/Await Deep Dive & Concurrency Basics

## Overview

In this task, I learned how asynchronous programming works in C# using Async/Await, Tasks, concurrency, and cancellation tokens.

The main goal of this task was to understand how to write non-blocking code, how asynchronous operations are executed, and how to improve performance by running independent operations concurrently.

## What I Learned

During this task, I learned about the Task-Based Asynchronous Pattern (TAP) and how a Task represents an asynchronous operation that may complete in the future.

I learned how to use `async` and `await` to write asynchronous methods and how awaiting a Task does not block the thread while waiting for the operation to complete.

I also learned why we should avoid blocking asynchronous code using `.Result` or `.Wait()` because they can block the thread and cause performance problems.

Another important concept I learned is running multiple independent operations concurrently using `Task.WhenAll`. Instead of waiting for each operation to finish before starting the next one, I can start all tasks together and wait for all of them to complete.

I also learned how to use `CancellationToken` and `CancellationTokenSource` to cancel long-running asynchronous operations when they are no longer needed.

## Task Description

The task was to create a console application that demonstrates asynchronous operations and concurrency.

The requirements were:

- Create three asynchronous methods that simulate different data sources.
- Use `Task.Delay()` to simulate long-running operations.
- Run the operations sequentially and measure the total execution time.
- Run the same operations concurrently using `Task.WhenAll`.
- Compare the execution time between sequential and concurrent execution.
- Add a cancellation example using `CancellationToken`.

## Task Implementation

I created three asynchronous methods to simulate different operations:

- Reading data from a database.
- Calling an API.
- Reading data from a file.

Each method uses `Task.Delay()` to simulate waiting for an external operation.

First, I executed the three operations sequentially using `await`. In this approach, each operation waits for the previous operation to complete before starting.

After that, I implemented concurrent execution by starting all tasks at the same time and using `Task.WhenAll()` to wait until all operations were completed.

I used `Stopwatch` to measure the execution time and compare the difference between sequential and concurrent execution.

The concurrent approach was faster because the independent operations could run at the same time instead of waiting for each other.

## Cancellation Implementation

I added a separate asynchronous method that accepts a `CancellationToken`.

I used `CancellationTokenSource` to create a cancellation request and passed the token to the asynchronous operation.

During execution, I called `Cancel()` to stop the running task and handled the cancellation using `OperationCanceledException`.

This demonstrated how long-running asynchronous operations can be cancelled safely.

## Technologies Used

- C#
- .NET
- Async/Await
- Task
- Task.WhenAll
- CancellationToken
- Stopwatch

## Conclusion

This task helped me understand the fundamentals of asynchronous programming in C#.

I learned how to create asynchronous methods, avoid blocking code, run multiple tasks concurrently, measure performance differences, and handle cancellation in async operations.
