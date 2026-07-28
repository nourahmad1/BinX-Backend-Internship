namespace LibrarySystem;

public class Book : IBorrowable
{ 
    private string title;
    private string author;

    public string Title
    {
        get { return title; }
        set { title = value; }
    }

    public string Author
    {
        get { return author; }
        set { author = value; }
    }

    public Book(string title, string author)
    {   if (string.IsNullOrWhiteSpace(title))
        {
            throw new Exception("Title cannot be null or empty.");
        }
        if (string.IsNullOrWhiteSpace(author))
        {
            throw new Exception("Author cannot be null or empty.");
        }
        this.title = title;
        this.author = author;
    }

    public void Borrow()
    {
        Console.WriteLine($"You have borrowed the book '{title}' by {author}.");
    }


}