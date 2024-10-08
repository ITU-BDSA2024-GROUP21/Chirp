namespace Chirp.Razor;

public class Author
{
    string Name { get; set; }
    string Email { get; set; }
    
    ICollection<Cheep> Cheeps { get; set; }
}