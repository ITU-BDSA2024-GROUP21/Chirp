namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheeps(int page);

    public Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page);
}

public class CheepRepository : ICheepRepository
{
    private readonly DBFacade facade;
    private static readonly List<CheepDTO> _cheeps = new();
    
    public CheepRepository(DBFacade facade)
    {
        this.facade = facade;
    }
    
    public async Task<List<CheepDTO>> GetCheeps(int page)
    {
        string sqlQuery = 
            @"SELECT user.username, message.text, message.pub_date From message
            JOIN user ON message.author_id = user.user_id
            ORDER BY message.pub_date DESC LIMIT 32 OFFSET @page";
        return facade.CheepQuery(sqlQuery, page);
    }
    
    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page)
    {
        string sqlQuery = 
            @"SELECT user.username, message.text, message.pub_date From message
            JOIN user ON message.author_id = user.user_id
            WHERE user.username = @author
            ORDER BY message.pub_date DESC LIMIT 32 OFFSET @page";
        return facade.CheepQuery(sqlQuery, page, author);
    }
}