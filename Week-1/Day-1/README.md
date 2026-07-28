# Day 1 - Environment Setup and First .NET Console Application

This repository contains my work and progress during the BinX Backend Internship program. The purpose of this internship is to learn backend development using .NET technologies, understand professional development workflows, and build practical skills through hands-on tasks and projects.

Today was my first day in the internship, and the main focus was setting up my development environment and getting familiar with the basic tools and workflow that will be used throughout the program.

I started by preparing my machine for .NET development. I installed the .NET SDK, which includes the .NET runtime, C# compiler, and the .NET CLI tools needed to create and manage applications. After installation, I verified that everything was configured correctly by running the command `dotnet --version`. This step confirmed that the SDK was installed successfully and that the system could recognize the .NET commands.

After setting up the environment, I learned about the .NET CLI and how it is used as the main interface for working with .NET projects. I practiced the basic commands that are commonly used in backend development. I learned that `dotnet new` is used to create new projects from templates, `dotnet build` is used to compile the project and check for errors, and `dotnet run` is used to build and execute the application directly.

I also configured my development environment using Visual Studio Code with the C# Dev Kit extension. This helped me enable important features such as IntelliSense, code suggestions, error detection, and debugging support, which will be important when working on larger backend applications in the future.

As my first practical task, I created a new console application using the command:

`dotnet new console -o HelloBinX`

This command generated a new .NET console project named HelloBinX. The project included the main source code file `Program.cs`, which contains the C# code that controls the application behavior, and the `HelloBinX.csproj` file, which contains the project configuration and information about the .NET framework being used.

After creating the project, I ran the application using:

`dotnet run`

The application successfully executed and displayed the default output:

`Hello, World!`

This confirmed that my .NET environment was working correctly and that I was able to create and run my first C# application.

After testing the default application, I modified the code inside `Program.cs` to create a simple personalized program. I changed the application so that it prints my name and today's date instead of the default message. After making the changes, I used:

`dotnet build`

to compile the project and make sure there were no errors, then I used:

`dotnet run`

again to execute the updated application and verify the result.

Through this task, I learned the basic lifecycle of a .NET application, starting from creating a project, understanding the project structure, writing C# code, building the application, running it, and preparing the project for version control.

The tools I used during Day 1 were:

- .NET SDK
- Visual Studio Code
- C# Dev Kit Extension
- PowerShell Terminal
- GitHub

By completing this first day, I gained a better understanding of how .NET projects are created and managed. This setup will be the foundation for the upcoming backend development tasks and larger projects that will be developed during the internship.
