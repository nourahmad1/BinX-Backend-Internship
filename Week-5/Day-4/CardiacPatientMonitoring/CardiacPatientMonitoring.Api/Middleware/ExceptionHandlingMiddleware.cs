using Microsoft.AspNetCore.Mvc;

namespace CardiacPatientMonitoring.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Continue with the normal request pipeline
            await _next(context);
        }
        catch (Exception exception)
        {
            // Log the full exception on the server
            _logger.LogError(
                exception,
                "An unhandled exception occurred while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(
                context,
                exception,
                _environment);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        IWebHostEnvironment environment)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Instance = context.Request.Path
        };

        // Show exception details only during development
        if (environment.IsDevelopment())
        {
            problemDetails.Detail = exception.Message;
        }
        else
        {
            // Never expose internal exception details in production
            problemDetails.Detail =
                "Please try again later.";
        }

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}