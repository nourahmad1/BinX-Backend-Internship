namespace GenericRepositoryLab;
public class Member
{
    public string Name { get; set; }
    public string Email { get; set; }
    public Member(string name, string email)
    {
        Name = name;
        Email = email;
    }
}