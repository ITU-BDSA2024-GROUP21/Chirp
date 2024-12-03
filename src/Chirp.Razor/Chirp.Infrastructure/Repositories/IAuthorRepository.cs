namespace Chirp.Infrastructure;

public interface IAuthorRepository
{
    public Task<Author> GetAuthorByName(string Name);
    public Task<Author> GetAuthorByEmail(string Email);
    public Task<Author> ConvertAuthors(AuthorDTO author);
    public Task<Author> CheckAuthorExists(AuthorDTO author);
    public Task DeleteAuthorByEmail(string email);
    public Task AddAuthorToDB(Author author);
}