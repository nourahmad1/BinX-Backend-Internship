namespace LibrarySystem;

    public class Book 
    {
        public string Title {get; set;}
        public string Author {get; set;}
        public bool IsBorrowed {get; set;}

        public Book (string title, string author, bool isBorrowed =false)
        {
        Title = title;
        Author = author;
        IsBorrowed = isBorrowed;
        }
        public void DisplayInfo()
        {
          Console.WriteLine($"Title: {Title}, Author: {Author}, Is Borrowed: {IsBorrowed}");
        }
    }

