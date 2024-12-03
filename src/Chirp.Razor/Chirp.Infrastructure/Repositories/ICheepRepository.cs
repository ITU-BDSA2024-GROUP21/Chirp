namespace Chirp.Infrastructure;

public interface ICheepRepository
{
    public Task<List<Cheep>> GetCheeps(int page);

    public Task<List<Cheep>> GetCheepsFromAuthor(string author, int page);

    public Task<Author> GetAuthorByName(string Name);
    public Task<List<Cheep>> GetCheepsFromAuthor1(string author);
    public Task<Author> GetAuthorByEmail(string Email);
    public Task<Author> ConvertAuthors(AuthorDTO author);
    public Task<Cheep> ConvertCheeps(CheepDTO cheep, AuthorDTO author);
    public Task DeleteCheep(int id);
    public Task<Author> CheckAuthorExists(AuthorDTO author);
    public Task FollowAuthor(int followingAuthorId, int followedAuthorId);
    public Task<List<string>> GetFollowedAuthorsAsync(int authorId);
    public Task<List<Cheep>> GetCheepsFromFollowedAuthorsAsync(IEnumerable<string> authors, int page);
    public Task<bool> IsFollowing(int followerId, int followedId);
    public Task Unfollow(int followingAuthorId, int followedAuthorId);
    public Task DeleteAuthorAndCheepsByEmail(string email);
    public Task<Bio> ConvertBio(BioDTO newBio, AuthorDTO newAuthor);
    public Task<Bio> GetBio(string author);
    public Task<bool> AuthorHasBio(string author);
    public Task DeleteBio(Author author);
    public Task AddAuthorAsync(Author author);

}