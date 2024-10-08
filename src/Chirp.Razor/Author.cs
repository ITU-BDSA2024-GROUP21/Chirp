namespace Chirp.Razor;

public class Author
{
    string Name { get; set; }
    string Email { get; set; }
    
    ICollection<Cheep> Cheeps { get; set; }
    
    public Author(string name, string email)
    {
        Name = name;
        Email = email;
        Cheeps = new List<Cheep>();
    }
}