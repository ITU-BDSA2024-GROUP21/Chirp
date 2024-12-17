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
    
    //This method is used to retrieve the author from the database, by using the name to search in the database
    public async Task<Author> GetAuthorByName(string Name)
    {
        var sqlQuery =  _chirpDbContext.Authors
            .Select(author => author)
            .Where(author => author.Name.ToLower() == Name.ToLower());
        
        var result = await sqlQuery.FirstOrDefaultAsync();
        return result ?? throw new InvalidOperationException();
    }
    
    // This method is used to retrieve the author from the database, when the email is available 
    public async Task<Author> GetAuthorByEmail(string Email)
    {
        var sqlQuery =  _chirpDbContext.Authors
            .Select(author => author)
            .Where(author => author.Email.ToLower() == Email.ToLower());
        
        var result = await sqlQuery.FirstOrDefaultAsync();
        return result!;
    }
    
    
    // This is for converting the author from an authorDTO to an author object
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

    // This is a method for checking if the author exists in the database
    public async Task<Author> CheckAuthorExists(AuthorDTO author)
    {
        var doesAuthorExist = await  _chirpDbContext.Authors.FirstOrDefaultAsync(a => a.Name == author.Name || a.Email == author.Email);
        if (doesAuthorExist == null)
        {
            return null!;
        }

        return doesAuthorExist;
    }
    
    // This method is for deleting the author from the database by searching for the email
    public async Task DeleteAuthorByEmail(string email)
    {
        // This is to delete the author from the Authors table
        var author = await _chirpDbContext.Authors.FirstOrDefaultAsync(a => a.Email == email);
        if (author == null)
        {
            Console.WriteLine($"Author with email {email} not found.");
            return;
        }
    
        // This is to delete the author from the AspNetUsers table
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var logins = _chirpDbContext.UserLogins.Where(l => l.UserId == user.Id);
            _chirpDbContext.RemoveRange(logins);
            await _userManager.DeleteAsync(user);
        }

        _chirpDbContext.Authors.Remove(author);
        
        // This is to delete all the cheeps from author that is being deleted
        var cheeps = _chirpDbContext.Cheeps.Where(c => c.Author == author);
        _chirpDbContext.Cheeps.RemoveRange(cheeps);

        // This is to delete the author from the AuthorFollows table, where the author is the one following other authors
        var following = _chirpDbContext.AuthorFollows.Where(a => a.Follower == author);
        _chirpDbContext.AuthorFollows.RemoveRange(following);
        
        // This is to delete the author from the AuthorFollows, where the author is the one being followed by other authors
        var followed = _chirpDbContext.AuthorFollows.Where(a => a.Following == author);
        _chirpDbContext.AuthorFollows.RemoveRange(followed);

        // This is to delete the Authors bio from the Bios table
        var bio = await _chirpDbContext.Bios.FirstOrDefaultAsync(b => b.AuthorId == author.AuthorId);
        if (bio != null)
        {
            _chirpDbContext.Bios.Remove(bio);
        }
        
        
        await _chirpDbContext.SaveChangesAsync();
    }
    
    // This is to add the respective author to the database
    public async Task AddAuthorToDB(Author author)
    {
        _chirpDbContext.Authors.Add(author);
        await _chirpDbContext.SaveChangesAsync();
    }
    
    // The following 3 methods are used for testing purposes only and does not reflect our refactoring
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
            .Select(f => f.Following!.Name)
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