using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Chirp.Infrastructure;


public class BioRepository : IBioRepository
{
    private readonly ChirpDBContext _chirpDbContext;
    public ChirpDBContext DbContext => _chirpDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    
    
    public BioRepository(ChirpDBContext chirpDbContext, UserManager<ApplicationUser> userManager)
    {
        _chirpDbContext = chirpDbContext;
        _userManager = userManager;
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
        if (FindBio != null)
        { 
            _chirpDbContext.Bios.Remove(FindBio);
        }
        
        await _chirpDbContext.SaveChangesAsync();
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