using MiddlewareDependencyInjectionApi.Middleware;
using MiddlewareDependencyInjectionApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Dependency Injection
builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Middleware Pipeline
app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

app.Run();