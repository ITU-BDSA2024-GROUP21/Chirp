namespace Chirp.Infrastructure;

public interface ICheepRepository
{
    public Task<List<Cheep>> GetCheeps(int page);

    public Task<List<Cheep>> GetCheepsFromAuthor(string author, int page);

    public Task<Author> GetAuthorByName(string Name);
    public Task<Author> GetAuthorByEmail(string Email);
    public Task<Author> ConvertAuthors(AuthorDTO author);
    public Task<Cheep> ConvertCheeps(CheepDTO cheep, AuthorDTO author);
    public Task DeleteCheep(int id);
    public Task FollowAuthor(int followingAuthorId, int followedAuthorId);
    public Task<List<string>> GetFollowedAuthorsAsync(int authorId);
    public Task<List<CheepDTO>> GetCheepsFromFollowedAuthorsAsync(IEnumerable<string> authors, int page);
    public Task<bool> IsFollowing(int followerId, int followedId);

}