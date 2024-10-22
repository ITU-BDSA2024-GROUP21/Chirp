namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<Cheep>> GetCheeps(int page);

    public Task<List<Cheep>> GetCheepsFromAuthor(string author, int page);

    public Task<Author> GetAuthorByName(string Name);
    public Task<Author> GetAuthorByEmail(string Email);
    public Task<Author> ConvertAuthors(AuthorDTO author);
    public Task ConvertCheeps(CheepDTO cheep, AuthorDTO author);

}