namespace Chirp.Razor;

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int page);

    public Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page);
}