using System.Data;
using Microsoft.Data.Sqlite;
using Chirp.Razor;


public class CheepService : ICheepService
{
    private readonly ICheepRepository _cheepRepository;
    
    public CheepService(ICheepRepository cheepRepository) {
        _cheepRepository = cheepRepository;
    }

    public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

    public async void CreateAuthor(string username, string email)
    {
        AuthorDTO newAuthor = new AuthorDTO
        {
            Name = username,
            Email = email
        };
        await _cheepRepository.CreateAuthors(newAuthor);
    }


    public async void CreateCheep(string username, string email, string message, string timestamp)
    {
        AuthorDTO newAuthor = new AuthorDTO
        {
            Name = username,
            Email = email,
        };
        CheepDTO newCheep = new CheepDTO
        {
            Author = username,
            Text = message,
            TimeStamp = timestamp
        };
        await _cheepRepository.CreateCheeps(newCheep, newAuthor);
    }

    public async Task<List<CheepDTO>> GetCheeps(int page)
    {
        var result = await _cheepRepository.GetCheeps(page);
        var cheeps = DTOConversion(result);
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