# Day 2 - C# Fundamentals I

## What I learned

Today I learned about the basics of C# types, variables, and control flow.

I learned that there are two main types of variables in C#:
- Value types store the value directly.
- Reference types store a reference to the object.

I practiced copying both types and saw that value types create a separate copy, while reference types can point to the same data.

I also learned how to create variables with clear names and how to use `var` when the type is already clear.

For control flow, I practiced:
- if statements
- switch expressions
- loops

I also learned about nullable reference types. They help me handle values that can be empty and prevent possible errors with null values.


## Task

In this task, I created a small C# console application to practice the fundamentals I learned.

The program demonstrates how different types of variables work in C#:

- Created and displayed different **value type variables** such as `int`, `double`, and `bool`.
- Created and displayed different **reference type variables** such as `string` and arrays.
- Practiced the difference between copying values and copying references:
  - Value types create a separate copy, so changing one variable does not affect the original variable.
  - Reference types point to the same object, so changing the copied reference can also change the original data.
- Built a grade calculator using a **switch expression** to convert a score into a grade description.
- Handled user input safely by checking if the entered value is `null` before using it.

This task helped me understand how C# stores data, how variables behave when copied, and how to write safer programs when dealing with user input.



## Tools Used

- C#
- .NET
- VS Code
- GitHub
