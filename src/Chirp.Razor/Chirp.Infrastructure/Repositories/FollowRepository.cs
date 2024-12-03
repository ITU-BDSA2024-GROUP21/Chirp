using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Chirp.Infrastructure;


public class FollowRepository : IFollowRepository
{
    private readonly ChirpDBContext _chirpDbContext;
    public ChirpDBContext DbContext => _chirpDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    
    
    public FollowRepository(ChirpDBContext chirpDbContext, UserManager<ApplicationUser> userManager)
    {
        _chirpDbContext = chirpDbContext;
        _userManager = userManager;
    }
    
    
    public async Task FollowAuthor(int followingAuthorId, int followedAuthorId)
    {
        var followRelation = new AuthorFollow
        {
            FollowerId = followingAuthorId,
            FollowingId = followedAuthorId
        };
        _chirpDbContext.AuthorFollows.Add(followRelation);

        await _chirpDbContext.SaveChangesAsync();
    }
    
    public async Task<List<String>> GetFollowedAuthors(int authorId)
    {
        return await _chirpDbContext.AuthorFollows
            .Where(f => f.FollowerId == authorId)
            .Select(f => f.following.Name)
            .ToListAsync();
    }

    public async Task<bool> IsFollowing(int followingAuthorId, int followedAuthorId)
    {
        return await _chirpDbContext.AuthorFollows
            .AnyAsync(f => f.FollowerId == followingAuthorId && f.FollowingId == followedAuthorId);
    }

    public async Task Unfollow(int followingAuthorId, int followedAuthorId)
    {
        
        var followRelation = await _chirpDbContext.AuthorFollows
            .FirstOrDefaultAsync(f => f.FollowerId == followingAuthorId && f.FollowingId == followedAuthorId);

        _chirpDbContext.AuthorFollows.Remove(followRelation!);
        await _chirpDbContext.SaveChangesAsync();
        
    }

}