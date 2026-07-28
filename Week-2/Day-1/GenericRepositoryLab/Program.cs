namespace GenericRepositoryLab;

class Program
{
    static void Main(string[] args)
    {
        Repository<Book> bookRepository = new();

        bookRepository.Add(new Book("C# in Depth", "Jon Skeet"));
        bookRepository.Add(new Book("Clean Code", "Robert C. Martin"));

        Console.WriteLine("Books:");

        foreach (Book book in bookRepository.GetAll())
        {
            Console.WriteLine($"{book.Title} by {book.Author}");
        }

        Book? foundBook = bookRepository.Find(book => book.Title == "Clean Code");

        if (foundBook != null)
        {
            Console.WriteLine($"\nFound Book: {foundBook.Title}");
        }

        Console.WriteLine();

        Repository<Member> memberRepository = new();

        memberRepository.Add(new Member("Noor", "noor@example.com"));
        memberRepository.Add(new Member("Ola", "Ola@example.com"));

        Console.WriteLine("Members:");

        foreach (Member member in memberRepository.GetAll())
        {
            Console.WriteLine($"{member.Name} - {member.Email}");
        }

        Member? foundMember = memberRepository.Find(member => member.Name == "Noor");

        if (foundMember != null)
        {
            Console.WriteLine($"\nFound Member: {foundMember.Name}");
        }

        Console.WriteLine();

        IReadOnlyList<Book> books = bookRepository.GetAll();

        // books.Add(new Book("New Book", "Author"));
        // This line would cause a compile-time error because
        // IReadOnlyList does not allow modification.
    }
}