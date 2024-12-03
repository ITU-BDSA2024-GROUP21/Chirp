using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Chirp.Infrastructure;


public class NootRepository : INootRepository
{
    private readonly ChirpDBContext _chirpDbContext;
    public ChirpDBContext DbContext => _chirpDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    
    
    public NootRepository(ChirpDBContext chirpDbContext, UserManager<ApplicationUser> userManager)
    {
        _chirpDbContext = chirpDbContext;
        _userManager = userManager;
    }
    
    public async Task<List<Cheep>> GetNoots(int page)
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
    
    public async Task DeleteNoot(int cheepId)
    {
        var cheep = await _chirpDbContext.Cheeps.FindAsync(cheepId);
        if (cheep != null)
        { 
            _chirpDbContext.Cheeps.Remove(cheep);
        }
        
        await _chirpDbContext.SaveChangesAsync();
        
    }
    
    
    public async Task<List<Cheep>> GetNootsFromAuthor(string author, int page)
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
    
    public async Task<List<Cheep>> GetNootsWithoutPage(string author)
    {
        var sqlQuery = _chirpDbContext.Cheeps
            .Where(cheep => cheep.Author.Name == author)
            .Select(cheep => cheep)
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp);

        var result = await sqlQuery.ToListAsync();
        return result;
    }

    
    public async Task<Cheep> ConvertNoots(CheepDTO cheeps, AuthorDTO author)
    {
        var _author = await ConvertAuthors(author);
        
        Cheep newCheep = new Cheep{ Author = _author, Text = cheeps.Text, TimeStamp = DateTime.UtcNow };
        
        await _chirpDbContext.Cheeps.AddAsync(newCheep);
        await _chirpDbContext.SaveChangesAsync();
        return newCheep;

    }
    
    
    public async Task<List<Cheep>> GetNootsFromFollowedAuthors(IEnumerable<string> followedAuthors, int page)
    {
        return await _chirpDbContext.Cheeps
            .Where(c => followedAuthors.Contains(c.Author.Name))
            .OrderByDescending(c => c.TimeStamp)
            .Skip(page * 32)
            .Take(32)
            .Include(c => c.Author)
            .Select(cheep => cheep)
            .ToListAsync();
    }
    
    //This is helping methods for converting author
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
    

}