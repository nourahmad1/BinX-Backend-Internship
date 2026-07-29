using Microsoft.AspNetCore.Mvc;

namespace MyFirstApi.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    [HttpGet]
    public string[] GetBooks()
    {
        return new[]
        {
            "C# Controller",
            "ASP.NET Core Controller",
            "SQL Controller"
        };
    }


    [HttpGet("{id}")]
    public string GetBookById(int id)
    {
        return $"Book number {id}";
    }
}