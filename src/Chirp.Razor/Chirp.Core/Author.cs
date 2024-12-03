using System.ComponentModel.DataAnnotations;
namespace Chirp.Core;

public class Author
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public int AuthorId { get; set; }
    public Bio? Bio { get; set; }
    
    [Required]
    public ICollection<Cheep> Cheeps { get; set; } = null!;

    [Required]
    public IEnumerable<AuthorFollow>? Followers { get; set; } 
    [Required]
    public IEnumerable<AuthorFollow>? Following { get; set; } 
}