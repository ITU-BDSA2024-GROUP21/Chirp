namespace Chirp.Infrastructure;

public interface IAuthorRepository
{
    public Task<Author> GetAuthorByName(string Name);
    public Task<Author> GetAuthorByEmail(string Email);
    public Task<Author> ConvertAuthors(AuthorDTO author);
    public Task<Author> CheckAuthorExists(AuthorDTO author);
    public Task DeleteAuthorByEmail(string email);
    public Task AddAuthorToDB(Author author);
    
    // These are used for testing purposes only and does not reflect our refactoring
    public Task FollowAuthor(int followingAuthorId, int followedAuthorId);
    public Task<List<string>> GetFollowedAuthors(int authorId);
    public Task Unfollow(int followingAuthorId, int followedAuthorId);
}