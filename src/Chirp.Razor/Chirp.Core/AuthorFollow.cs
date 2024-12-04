using System.ComponentModel.DataAnnotations;

namespace Chirp.Core;

public class AuthorFollow
{
    [Required]
    public required int FollowerId { get; set; }
    public Author? Follower { get; set; }
    
    [Required]
    public required int FollowingId { get; set; }
    public Author? Following { get; set; }
}