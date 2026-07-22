namespace LibrarySystem;
public class Member
{
    public string Name {get; set;}
    public string Email {get; set;}
    public Member(string name, string email)
    {
        Name = name;
        Email = email;
    }
    public void Borrow(Book book)
    {
        if (!book.IsBorrowed)
        {
            book.IsBorrowed = true;
            Console.WriteLine($"{Name} has borrowed the book: {book.Title}");
        }
        else
        {
            Console.WriteLine($"Sorry, {book.Title} is already borrowed.");
        }
    }
}