namespace Chirp.Infrastructure;

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int page);

    public Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page);
    public Task<Cheep> CreateCheep(string username, string email, string message, string timestamp, int id);
    public Task<Author> GetAuthorByName(string name);

}