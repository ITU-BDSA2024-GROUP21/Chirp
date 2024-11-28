using System.ComponentModel.DataAnnotations;

namespace Chirp.Core;

public class Bio
{
    [Required]
    [StringLength(300)]
    public required string Text { get; set; }
    [Required]
    public int BioId { get; set; }
    [Required]
    public required Author Author { get; set; }
    public int AuthorId { get; set; }

}