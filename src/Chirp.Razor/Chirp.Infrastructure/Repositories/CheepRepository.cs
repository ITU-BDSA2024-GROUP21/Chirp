using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;


public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _chirpDbContext;
    
    public CheepRepository(ChirpDBContext chirpDbContext)
    {
        _chirpDbContext = chirpDbContext;
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

    public async Task ConvertCheeps(CheepDTO cheeps, AuthorDTO author)
    {
        var _author = await ConvertAuthors(author);
        
        Cheep newCheep = new Cheep{ Author = _author, Text = cheeps.Text, TimeStamp = DateTime.UtcNow };
        
        await _chirpDbContext.Cheeps.AddAsync(newCheep);
        await _chirpDbContext.SaveChangesAsync();

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
}