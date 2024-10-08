namespace Chirp.Razor;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepRepository
{
    public List<CheepViewModel> GetCheeps(int page);

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
}

public class CheepRepository : ICheepRepository
{
    private readonly DBFacade facade;
    private static readonly List<CheepViewModel> _cheeps = new();
    
    public CheepRepository(DBFacade facade)
    {
        this.facade = facade;
    }
    
    
    
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
}