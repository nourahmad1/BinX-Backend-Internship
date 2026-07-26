namespace GenericRepositoryLab;
class Program
{
    static void Main(string[] args)
    {
      Repository <Book> bookRepository = new ();
        bookRepository.Add(new Book("C# in Depth", "Jon Skeet"));   
        bookRepository.Add(new Book("Clean Code", "Robert C. Martin"));
        Console.WriteLine("Books:"); 
        foreach (Book book in bookRepository.GetAll())
        {
            Console.WriteLine($" - {book.Title} by {book.Author}");
        }
        Book? foundBook = bookRepository.Find(b => b.Title == "Clean Code");
        if (foundBook != null)
        {
            Console.WriteLine($"Found book: {foundBook.Title} by {foundBook.Author}");
        }
        else
        {
            Console.WriteLine("Book not found.");
        }
        Console.WriteLine();
        Repository <Member> memberRepository = new ();
        memberRepository.Add(new Member("Nour","nour@gmail.com"));
        memberRepository.Add(new Member("Ola","ola@gmail.com"));
        Console.WriteLine("Members:");
        foreach (Member member in memberRepository.GetAll())
        {
            Console.WriteLine($" - {member.Name} ({member.Email})");
        }
        Member? foundMember = memberRepository.Find(m => m.Name == "Nour");
        if (foundMember != null)
        {
            Console.WriteLine($"Found member: {foundMember.Name} ({foundMember.Email})");
        }
        else
        {
            Console.WriteLine("Member not found.");
        }
        Console.WriteLine();
        IReadOnlyList<Book> books = bookRepository.GetAll();
    }   
 }
