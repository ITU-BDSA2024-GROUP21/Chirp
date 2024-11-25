﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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

    public async Task DeleteAuthorByEmail(string email)
    {
        var author = await _chirpDbContext.Authors.FirstOrDefaultAsync(a => a.Email == email);
        if (author == null)
        {
            Console.WriteLine($"Author with email {email} not found.");
            return;
        }

        _chirpDbContext.Authors.Remove(author);

        await _chirpDbContext.SaveChangesAsync();
    }
    
    public async Task DeleteAuthorAndCheeps(Author author)
    {
        if (author == null) throw new ArgumentNullException(nameof(author));
        
        var auth = await _chirpDbContext.Authors.FindAsync(author.AuthorId);
        if (auth != null)
        {
            //This remoces all the cheeps from the author from the database
            var cheeps = _chirpDbContext.Cheeps.Where(c => c.Author == auth);
            _chirpDbContext.Cheeps.RemoveRange(cheeps);
            
            //This removes the author from the database
            _chirpDbContext.Authors.Remove(auth);
            
            var user = await _userManager.FindByIdAsync(auth.AuthorId.ToString());
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

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
}