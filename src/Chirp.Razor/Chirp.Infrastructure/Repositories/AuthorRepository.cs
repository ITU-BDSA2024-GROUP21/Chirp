using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Chirp.Infrastructure;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDBContext _chirpDbContext;
    public ChirpDBContext DbContext => _chirpDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    
    
    public AuthorRepository(ChirpDBContext chirpDbContext, UserManager<ApplicationUser> userManager)
    {
        _chirpDbContext = chirpDbContext;
        _userManager = userManager;
    }
    public async Task<Author> GetAuthorByName(string Name)
    {
        var sqlQuery =  _chirpDbContext.Authors
            .Select(author => author)
            .Where(author => author.Name.ToLower() == Name.ToLower());
        
        var result = await sqlQuery.FirstOrDefaultAsync();
        return result ?? throw new InvalidOperationException();
    }
    
    
    public async Task<Author> GetAuthorByEmail(string Email)
    {
        var sqlQuery =  _chirpDbContext.Authors
            .Select(author => author)
            .Where(author => author.Email.ToLower() == Email.ToLower());
        
        var result = await sqlQuery.FirstOrDefaultAsync();
        return result!;
    }
    
    public async Task<Author> ConvertAuthors(AuthorDTO author)
    {
        var _author = await CheckAuthorExists(author);

        if (_author == null!)
        {
            Author newAuthor = new()
            {
                Name = author.Name,
                Email = author.Email,
                Cheeps = null!
            };
            var contextAuthor =await _chirpDbContext.Authors.AddAsync(newAuthor);

            await _chirpDbContext.SaveChangesAsync();
            return contextAuthor.Entity;
        }

        return _author;
    }

    public async Task<Author> CheckAuthorExists(AuthorDTO author)
    {
        var doesAuthorExist = await  _chirpDbContext.Authors.FirstOrDefaultAsync(a => a.Name == author.Name || a.Email == author.Email);
        if (doesAuthorExist == null)
        {
            return null!;
        }

        return doesAuthorExist;
    }
    
    public async Task DeleteAuthorByEmail(string email)
    {
        var author = await _chirpDbContext.Authors.FirstOrDefaultAsync(a => a.Email == email);
        if (author == null)
        {
            Console.WriteLine($"Author with email {email} not found.");
            return;
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }

        _chirpDbContext.Authors.Remove(author);
        
        var cheeps = _chirpDbContext.Cheeps.Where(c => c.Author == author);
        _chirpDbContext.Cheeps.RemoveRange(cheeps);

        var following = _chirpDbContext.AuthorFollows.Where(a => a.Follower == author);
        _chirpDbContext.AuthorFollows.RemoveRange(following);
        
        var followed = _chirpDbContext.AuthorFollows.Where(a => a.following == author);
        _chirpDbContext.AuthorFollows.RemoveRange(followed);

        var bio = await _chirpDbContext.Bios.FirstOrDefaultAsync(b => b.AuthorId == author.AuthorId);
        if (bio != null)
        {
            _chirpDbContext.Bios.Remove(bio);
        }

        
        Console.WriteLine(author.AuthorId);
        /*var follows = await _cheepService.GetFollowedAuthors(author.AuthorId);

        foreach (var follow in follows)
        {
            var followingAuthor = await GetAuthorByName(follow);
            var followingId = followingAuthor.AuthorId;
            var authorfollow = new AuthorFollow { FollowerId = author.AuthorId, FollowingId = followingId };
            _chirpDbContext.AuthorFollows.Remove(authorfollow);
        }*/
        
        await _chirpDbContext.SaveChangesAsync();
    }
    
    public async Task AddAuthorToDB(Author author)
    {
        _chirpDbContext.Authors.Add(author);
        await _chirpDbContext.SaveChangesAsync();
    }
    
    // These are used for testing purposes only and does not reflect our refactoring
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
    
    public async Task Unfollow(int followingAuthorId, int followedAuthorId)
    {
        
        var followRelation = await _chirpDbContext.AuthorFollows
            .FirstOrDefaultAsync(f => f.FollowerId == followingAuthorId && f.FollowingId == followedAuthorId);

        _chirpDbContext.AuthorFollows.Remove(followRelation!);
        await _chirpDbContext.SaveChangesAsync();
        
    }
}