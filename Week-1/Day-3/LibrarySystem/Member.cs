namespace LibrarySystem;
public class Member : IBorrowable
{
    private string email;
    public string Name { get; private set; }

    public string Email
    {
        get { return email; }
      
    }
    public Member(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Name cannot be null or empty.");
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new Exception("Email cannot be null or empty.");
        }
        Name = name;
        this.email = email;
    }
    public void Borrow()
    {
        Console.WriteLine($"Member '{Name}' with email '{email}' has borrowed a book.");
    }
}