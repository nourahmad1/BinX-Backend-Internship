using LibrarySystem;
class Program
{
  static async Task Main(string[] args)
    {
        List < Book> books = new List<Book>
        {
            new Book("C#", "Microsoft", true),
            new Book("Clean Code ", "Robert "),
            new Book("The Hobbit", "Tolkien", true),
            new Book("Atomic Habits", "James Clear"),
            new Book("The Pragmatic Programmer", "Andrew Hunt"),
            new Book("ASP.NET Core", "Microsoft"),
            new Book("Design Patterns", "GOF", true),
            new Book("Algorithms", "Robert Sedgewick"),
        };
        Console.WriteLine("List of books:");
        foreach (Book book in books)
        {
            book.DisplayInfo();
        }

          Console.WriteLine() ;
          var borrowedBooks = books.Where(b => b.IsBorrowed).ToList();
          Console.WriteLine("List of borrowed books:");
          foreach (Book book1 in borrowedBooks)  
            {
                Console.WriteLine(book1.Title);
            }
        Console.WriteLine() ;
        var titles = books.Select(b => b.Title).ToList();
        Console.WriteLine("List of book titles:");
        foreach (string title in titles)
        {
            Console.WriteLine(title);
        }
        Console.WriteLine() ;
        int borrowedCount = books.Count(b => b.IsBorrowed);
        Console.WriteLine($"Number of borrowed books: {borrowedCount}");
        Console.WriteLine() ;
        string message = await LoadLibraryMessage();
        Console.WriteLine(message);
        Console.Write("Enter you library card number: "); 
        try
        {
            int cardNumber = int.Parse(Console.ReadLine());
            Console.WriteLine($"Your library card number is: {cardNumber}");
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }

        static async Task<string> LoadLibraryMessage()
        {
            await Task.Delay(2000); 
            return "Library system loaded successfully!";
        }
       
    }
}