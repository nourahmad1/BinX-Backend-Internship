using Microsoft.AspNetCore.Mvc;
using MiddlewareDependencyInjectionApi.Services;

namespace MiddlewareDependencyInjectionApi.Controllers;

[ApiController]
[Route("books")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // GET /books
    [HttpGet]
    public IActionResult GetBooks()
    {
        return Ok(_bookService.GetBooks());
    }

    // GET /books/1
    [HttpGet("{id}")]
    public IActionResult GetBookById(int id)
    {
        return Ok($"Book number {id}");
    }
}