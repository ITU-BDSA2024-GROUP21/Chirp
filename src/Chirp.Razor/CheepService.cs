using System.Data;
using Microsoft.Data.Sqlite;
using Chirp.Razor;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int page);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
}

public class CheepService : ICheepService
{
    private readonly DBFacade facade;
    
    public CheepService(DBFacade facade)
    {
        this.facade = facade;
    }
    
    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel> _cheeps = new();

    public List<CheepViewModel> GetCheeps(int page)
    {
        string sqlQuery = 
            @"SELECT user.username, message.text, message.pub_date From message
            JOIN user ON message.author_id = user.user_id
            ORDER BY message.pub_date DESC LIMIT 32 OFFSET @page";
        return facade.CheepQuery(sqlQuery, page);
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        string sqlQuery = 
            @"SELECT user.username, message.text, message.pub_date From message
            JOIN user ON message.author_id = user.user_id
            WHERE user.username = @author
            ORDER BY message.pub_date DESC LIMIT 32 OFFSET @page";
        return facade.CheepQuery(sqlQuery, page, author);
    }

    public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
