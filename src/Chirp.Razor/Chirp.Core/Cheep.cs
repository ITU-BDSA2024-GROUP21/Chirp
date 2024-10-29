using System.ComponentModel.DataAnnotations;

namespace Chirp.Core;

public class Cheep
{
    [Required]
    [StringLength(160)]
    public required string Text { get; set; }
    public DateTime TimeStamp { get; set; }
    [Required]
    public int CheepId { get; set; }
    [Required]
    public required Author Author { get; set; }
    public int AuthorId { get; set; }

}