namespace MiddlewareDependencyInjectionApi.Services;

public class BookService : IBookService
{
    public string[] GetBooks()
    {
        return new[]
        {
            "C#",
            "ASP.NET Core",
            "SQL"
        };
    }
}