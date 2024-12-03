namespace Chirp.Infrastructure;

public interface INootRepository
{
    public Task<List<Cheep>> GetNoots(int page);
    public Task<List<Cheep>> GetNootsFromAuthor(string author, int page);
    public Task<List<Cheep>> GetNootsWithoutPage(string author);
    public Task<Cheep> ConvertNoots(CheepDTO cheep, AuthorDTO author);
    public Task DeleteNoot(int id);
    public Task<List<Cheep>> GetNootsFromFollowedAuthors(IEnumerable<string> authors, int page);
}