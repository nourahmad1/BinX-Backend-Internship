using System;
class Program
{
    static void Main(string [] args)
    {
        int age =20;
        double price =50.5;
        bool student =true;
        
        string name = "Ali";
        string city= "Nablus";
        int [ ] numbers = { 1,2,3};
        
        Console.WriteLine (age.GetType());
        Console.WriteLine (price.GetType());
        Console.WriteLine (student.GetType());
        Console.WriteLine (name.GetType());
        Console.WriteLine (city.GetType());
        Console.WriteLine (numbers.GetType());

        CopyBehavior();
        Console.Write("Enter score: ");
        int score= int.Parse(Console.ReadLine());
        Console.WriteLine(DescribeGrade(score));

        Console.WriteLine("Enter Your name: ");
        string? user= Console.ReadLine();
        if (user == null)
        {
            Console.WriteLine("No name");
        }
        else
        {
            Console.WriteLine("Hello " + user);
        }


    }

    static void CopyBehavior()
    {
      int x=5;
      int y=x;
     Console.WriteLine ("Before: ");
     Console.WriteLine ("x = "+ x);
     Console.WriteLine ("y = "+ y);
     y=10;
     Console.WriteLine ("After: ");
     Console.WriteLine ("x = "+ x);
     Console.WriteLine ("y = "+ y);

     int[] first ={ 1,2,3 };
     int[] second = first;
     Console.WriteLine ("Before: ");
     Console.WriteLine (" First = "+ first[0]);
     Console.WriteLine (" Second = "+ second[0]);
     second[0]=9;
    Console.WriteLine ("After: ");
     Console.WriteLine (" First = "+ first[0]);
     Console.WriteLine (" Second = "+ second[0]);

    }

    static string DescribeGrade (int score)
    { 
        return score switch
        {
            >= 90 =>  "Excellent",
            >= 70 =>  "Very Good",
            >= 50 =>  "Good",
                _ =>  "Failed",

        };

    }

}