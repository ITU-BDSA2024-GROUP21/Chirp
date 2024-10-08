﻿using Microsoft.EntityFrameworkCore;
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
}