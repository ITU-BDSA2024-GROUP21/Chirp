namespace Chirp.Core;

public class CheepDTO
{
    public required string Author { get; set; }
    public required string Text { get; set; }
    public required string TimeStamp { get; set; }
    public required int CheepId { get; set; }
    public required int AuthorId { get; set; }

}