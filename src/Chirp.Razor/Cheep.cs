namespace Chirp.Razor;

public class Cheep
{
    string Text { get; set; }
    DateTime Timestamp { get; set; }
    
    Author author { get; set; }
    
    public Cheep(string text, DateTime timestamp, Author author)
    {
        Text = text;
        Timestamp = timestamp;
        this.author = author;
        author.cheeps.Add(this);
        
    }
    
}