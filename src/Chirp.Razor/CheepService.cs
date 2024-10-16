using System.Data;
using Microsoft.Data.Sqlite;
using Chirp.Razor;


public class CheepService
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

}
