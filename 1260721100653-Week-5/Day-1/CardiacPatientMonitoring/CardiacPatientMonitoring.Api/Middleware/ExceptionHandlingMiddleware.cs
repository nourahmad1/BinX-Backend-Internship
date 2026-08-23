using System.Net;
using System.Text.Json;

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
            // Log the error so we can check it later
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
        // Return 500 when an unexpected error happens
        context.Response.StatusCode =
            (int)HttpStatusCode.InternalServerError;

        context.Response.ContentType =
            "application/json";

        // Show more details while developing the API
        if (environment.IsDevelopment())
        {
            var response = new Dictionary<string, object>
            {
                ["statusCode"] = 500,
                ["message"] = exception.Message,
                ["detail"] = exception.ToString()
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));

            return;
        }

        // Don't expose exception details in production
        var productionResponse = new Dictionary<string, object>
        {
            ["statusCode"] = 500,
            ["message"] =
                "An unexpected error occurred. Please try again later."
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(productionResponse));
    }
}