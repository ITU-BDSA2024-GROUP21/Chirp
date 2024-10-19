namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheeps(int page);

    public Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page);

    public Task<Author> GetAuthorByName(string Name);
    public Task<Author> GetAuthorByEmail(string Email);
    public Task<Author> CreateAuthors(AuthorDTO author);
    public Task CreateCheeps(CheepDTO cheep, AuthorDTO author);

}