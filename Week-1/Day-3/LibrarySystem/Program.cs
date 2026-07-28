namespace LibrarySystem;
class Program
{
    static void Main(string[] args)
    {
        Book book = new Book("C#", "Microsoft");
        Member member = new Member("Nour","nour@example.com");

        Borrow(book);
        Borrow(member);
        BorrowRequest request = new BorrowRequest("nour", "C#");
        Console.WriteLine($"Borrow Request: {request.MemberName} wants {request.BookTitle}");
    
    }   

    static void Borrow(IBorrowable item)
    {
        item.Borrow();
    }
}

