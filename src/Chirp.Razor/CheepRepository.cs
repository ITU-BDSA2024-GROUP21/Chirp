using Microsoft.EntityFrameworkCore;
using Chirp.Razor.Pages;

namespace Chirp.Razor;


public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _chirpDbContext;
    
    public CheepRepository(ChirpDBContext chirpDbContext)
    {
        _chirpDbContext = chirpDbContext;
    }
    
    public async Task<List<CheepDTO>> GetCheeps(int page)
    {
        var sqlQuery = _chirpDbContext.Cheeps
            .Select(cheep => cheep)
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32);

        List<Cheep> result = await sqlQuery.ToListAsync();
        var cheeps = DTOConversion(result);
        return cheeps;
    }
    
    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page)
    {
        var sqlQuery = _chirpDbContext.Cheeps
            .Where(cheep => cheep.Author.Name == author)    
            .Select(cheep => cheep)
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32);

        List<Cheep> result = await sqlQuery.ToListAsync();
        var cheeps = DTOConversion(result);
        return cheeps;
    }

    private static List<CheepDTO> DTOConversion(List<Cheep> cheeps)
    {
        var list = new List<CheepDTO>();
        foreach (var cheep in cheeps)
        {
            list.Add(new CheepDTO
            {
                Author = cheep.Author.Name,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString()
            });
        }
        return list;
    }

    public async Task<Author> GetAuthorByName(string Name)
    {
        var sqlQuery =  _chirpDbContext.Authors
            .Select(author => author)
            .Where(author => author.Name.ToLower() == Name.ToLower());
        
        var result = await sqlQuery.FirstOrDefaultAsync();
        return result;
    }
    
    public async Task<Author> GetAuthorByEmail(string Email)
    {
        var sqlQuery =  _chirpDbContext.Authors
            .Select(author => author)
            .Where(author => author.Email.ToLower() == Email.ToLower());
        
        var result = await sqlQuery.FirstOrDefaultAsync();
        return result;
    }

    public async Task<Author> CreateAuthors(AuthorDTO author)
    {
        var check = await CheckAuthorExists(author);

        if (check == null)
        {
            Author newAuthor = new() { Name = author.Name, Email = author.Email };
            var g =await _chirpDbContext.Authors.AddAsync(newAuthor);

            await _chirpDbContext.SaveChangesAsync();
            return g.Entity;
        }

        return check;
    }

    public async Task CreateCheeps(CheepDTO cheeps, AuthorDTO author)
    {
        var a = await CreateAuthors(author);
        
        Cheep newCheep = new Cheep{ Author = a, Text = cheeps.Text, TimeStamp = DateTime.UtcNow };
        
        await _chirpDbContext.Cheeps.AddAsync(newCheep);
        await _chirpDbContext.SaveChangesAsync();

    }
    
    private async Task<Author> CheckAuthorExists(AuthorDTO author)
    {
        var doesAuthorExist = await  _chirpDbContext.Authors.FirstOrDefaultAsync(a => a.Name == author.Name || a.Email == author.Email);
        if (doesAuthorExist == null)
        {
            return null;
        }

        return doesAuthorExist;
    }
}