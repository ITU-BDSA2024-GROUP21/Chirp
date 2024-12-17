namespace Chirp.Core;

public class BioDTO
{
    public required string Author { get; set; }
    public required string Text { get; set; }
    public required int BioId { get; set; }
    public required int AuthorId { get; set; }

}