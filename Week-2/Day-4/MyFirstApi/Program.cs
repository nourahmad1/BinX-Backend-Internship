var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/books", () =>
{
    return new[]
    {
        "C#",
        "ASP.NET Core",
        "SQL"
    };
});
app.MapGet("/books/{id}", (int id) =>
{
    return $"Book number {id}";
});

app.Run();