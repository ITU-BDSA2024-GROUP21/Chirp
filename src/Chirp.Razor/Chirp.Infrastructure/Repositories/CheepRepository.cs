using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Chirp.Infrastructure;


public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _chirpDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    
    
    public CheepRepository(ChirpDBContext chirpDbContext, UserManager<ApplicationUser> userManager)
    {
        _chirpDbContext = chirpDbContext;
        _userManager = userManager;
    }
    
    public async Task<List<Cheep>> GetCheeps(int page)
    {
        var sqlQuery = _chirpDbContext.Cheeps
            .Select(cheep => cheep)
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip(page * 32)
            .Take(32);

        var result = await sqlQuery.ToListAsync();
        return result;
    }
    
    public async Task DeleteCheep(int cheepId)
    {
        var cheep = await _chirpDbContext.Cheeps.FindAsync(cheepId);
        if (cheep != null)
        { 
            _chirpDbContext.Cheeps.Remove(cheep);
        }
        
        await _chirpDbContext.SaveChangesAsync();
        
    }

    public async Task DeleteAuthorAndCheepsByEmail(string email)
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
        
        Console.WriteLine(author.AuthorId);
        var follows = await GetFollowedAuthorsAsync(author.AuthorId);

        foreach (var follow in follows)
        {
            var followingAuthor = await GetAuthorByName(follow);
            var followingId = followingAuthor.AuthorId;
            var authorfollow = new AuthorFollow { FollowerId = author.AuthorId, FollowingId = followingId };
            _chirpDbContext.AuthorFollows.Remove(authorfollow);
        }
        
        await _chirpDbContext.SaveChangesAsync();
    }
    
    
    
    public async Task<List<Cheep>> GetCheepsFromAuthor(string author, int page)
    {
        var sqlQuery = _chirpDbContext.Cheeps
            .Where(cheep => cheep.Author.Name == author)    
            .Select(cheep => cheep)
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip(page * 32)
            .Take(32);

        var result = await sqlQuery.ToListAsync();
        return result;
    }
    
    public async Task<List<Cheep>> GetCheepsFromAuthor1(string author)
    {
        var sqlQuery = _chirpDbContext.Cheeps
            .Where(cheep => cheep.Author.Name == author)
            .Select(cheep => cheep)
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp);

        var result = await sqlQuery.ToListAsync();
        return result;
    }


    public async Task<Author> GetAuthorByName(string Name)
    {
        var sqlQuery =  _chirpDbContext.Authors
            .Select(author => author)
            .Where(author => author.Name.ToLower() == Name.ToLower());
        
        var result = await sqlQuery.FirstOrDefaultAsync();
        return result ?? throw new InvalidOperationException();
    }
    
    public async Task<string?> GetEmailByAuthorIdAsync(int authorId)
    {
        var author = await _chirpDbContext.Authors
            .FirstOrDefaultAsync(a => a.AuthorId == authorId);

        return author?.Email;
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

    public async Task<Cheep> ConvertCheeps(CheepDTO cheeps, AuthorDTO author)
    {
        var _author = await ConvertAuthors(author);
        
        Cheep newCheep = new Cheep{ Author = _author, Text = cheeps.Text, TimeStamp = DateTime.UtcNow };
        
        await _chirpDbContext.Cheeps.AddAsync(newCheep);
        await _chirpDbContext.SaveChangesAsync();
        return newCheep;

    }
    
    private async Task<Author> CheckAuthorExists(AuthorDTO author)
    {
        var doesAuthorExist = await  _chirpDbContext.Authors.FirstOrDefaultAsync(a => a.Name == author.Name || a.Email == author.Email);
        if (doesAuthorExist == null)
        {
            return null!;
        }

        return doesAuthorExist;
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
    
    public async Task<List<String>> GetFollowedAuthorsAsync(int authorId)
    {
        return await _chirpDbContext.AuthorFollows
            .Where(f => f.FollowerId == authorId)
            .Select(f => f.following.Name)
            .ToListAsync();
    }

    public async Task<List<Cheep>> GetCheepsFromFollowedAuthorsAsync(IEnumerable<string> followedAuthors, int page)
    {
        return await _chirpDbContext.Cheeps
            .Where(c => followedAuthors.Contains(c.Author.Name))
            .OrderByDescending(c => c.TimeStamp)
            .Skip(page * 32)
            .Take(32)
            .Select(cheep => cheep)
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
    
    public async Task<Bio> ConvertBio(BioDTO bio, AuthorDTO author)
    {
        var _author = await ConvertAuthors(author);
        
        Bio newBio = new Bio { Author = _author, Text = bio.Text, BioId = bio.BioId, AuthorId = _author.AuthorId };
        
        
        await _chirpDbContext.Bios.AddAsync(newBio);
        await _chirpDbContext.SaveChangesAsync();
        return newBio;

    }
    
    public async Task<Bio> GetBio(string author)
    {
        var sqlQuery = _chirpDbContext.Bios
            .Where(bio => bio.Author.Name == author)
            .Select(bio => bio)
            .Include(bio => bio.Author);

        var result = await sqlQuery.FirstOrDefaultAsync();
        return result;
    }

    public async Task<bool> AuthorHasBio(string author)
    {
        if (await _chirpDbContext.Bios.AnyAsync(bio => bio.Author.Name == author))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task DeleteBio(Author author)
    {
        var FindBio = await _chirpDbContext.Bios.FirstOrDefaultAsync(b => b.AuthorId == author.AuthorId);
        Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" + author.AuthorId);
        if (FindBio != null)
        { 
            _chirpDbContext.Bios.Remove(FindBio);
        }
        
        await _chirpDbContext.SaveChangesAsync();
    }

}
