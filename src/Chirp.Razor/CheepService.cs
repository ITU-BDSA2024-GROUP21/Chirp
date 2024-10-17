using System.Data;
using Microsoft.Data.Sqlite;
namespace Chirp.Razor;


public class CheepService : ICheepService
{
    private readonly ICheepRepository _cheepRepository;
    
    public CheepService(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }

    public async Task<List<CheepDTO>> GetCheeps(int page)
    {
        var result = await _cheepRepository.GetCheeps(page);
        var cheeps =  DTOConversion(result);
        return cheeps;
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page)
    {
        var result = await _cheepRepository.GetCheepsFromAuthor(author, page);
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
