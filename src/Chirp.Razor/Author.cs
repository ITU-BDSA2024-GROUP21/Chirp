using System.ComponentModel.DataAnnotations;

namespace Chirp.Razor;

public class Author
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public int AuthorId { get; set; }

   [Required]
    public required ICollection<Cheep> Cheeps { get; set; }
}

// Note har ikke ændret andet men undre mig over de her 3 required fjerner dem alle
// required make sure that a property cannot be null after it's created