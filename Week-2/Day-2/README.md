# Day 2 - Advanced LINQ & Deferred Execution

## Overview

In this task, I learned advanced LINQ concepts in C# and practiced how to work with collections in a cleaner and more efficient way.

The main focus was understanding how LINQ queries work, how to group and combine data, how to flatten nested collections, and how execution timing can affect the result of a query.

---

## What I Learned

### Deferred Execution and Immediate Execution

I learned that LINQ queries do not always execute immediately.

Methods like:

- `Where()`
- `Select()`
- `OrderBy()`

use **Deferred Execution**, which means the query is created first but it runs only when the result is needed, for example when using:

- `foreach`
- `ToList()`
- `Count()`

I also learned that some methods force the query to execute immediately, such as:

- `ToList()`
- `ToArray()`
- `Count()`

This is called **Immediate Execution**.

I practiced how changing the original collection before executing a deferred query can affect the final result.

---

### GroupBy

I learned how to use `GroupBy()` to group data based on a specific key.

In this task, I grouped orders by customer ID and calculated the total amount of orders for each customer using `Sum()`.

This is useful when creating summaries and reports from data.

---

### Join

I learned how to combine two related collections using `Join()`.

I connected the Customers collection with the Orders collection using the relationship between:

```
Customer.Id
=
Order.CustomerId
```

This allowed me to display customer names together with their order amounts.

---

### SelectMany

I learned how to use `SelectMany()` to flatten nested collections.

Instead of having multiple lists inside another collection, `SelectMany()` combines all inner elements into one single sequence.

Example:

Before:

```
Order 1
 - Laptop
 - Mouse

Order 2
 - Keyboard
```

After:

```
Laptop
Mouse
Keyboard
```

---

### LINQ Performance Considerations

I learned that using `ToList()` too early can cause unnecessary memory usage because it executes the query before finishing all filtering operations.

I also learned that running the same deferred query multiple times can repeat the same work, so sometimes storing the result using `ToList()` is more efficient.

---

# Task Implementation

For the hands-on lab, I created a simple Customer and Order system using C#.

The project contains:

- Customer class
- Order class
- Program class

The Customer and Order collections are connected using `CustomerId` as a foreign key.

---

## Implemented Requirements

### 1. Related Collections

I created two related collections:

### Customers

Contains:

- Id
- Name

### Orders

Contains:

- Id
- CustomerId
- Amount
- Items

The `CustomerId` property connects each order with its customer.

---

### 2. GroupBy Implementation

I used `GroupBy()` to group orders by customer and calculate the total amount spent by each customer.

Example result:

```
Customer Id: 1, Total: 150
Customer Id: 2, Total: 280
Customer Id: 3, Total: 150
```

---

### 3. Join Implementation

I used `Join()` to combine customer names with their order amounts.

Example result:

```
Noor ordered 100
Noor ordered 50
Ola ordered 200
Ola ordered 80
```

---

### 4. SelectMany Implementation

I added items inside each order and used `SelectMany()` to get all items from all orders as one list.

Example:

```
Laptop
Mouse
Keyboard
Phone
```

---

### 5. Deferred Execution Demonstration

I created a LINQ query and modified the original collection before executing it.

The new value appeared in the result because the query was executed later during enumeration.

Example:

```
4
5
10
```

This demonstrated how deferred execution works.

---

## Technologies Used

- C#
- .NET SDK
- LINQ
- Visual Studio Code

---

## Conclusion

Through this task, I improved my understanding of advanced LINQ operations and learned how to use LINQ to filter, group, combine, and transform data.

I also learned the difference between deferred and immediate execution and how execution timing can affect the result of a query.
