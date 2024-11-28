namespace Chirp.Infrastructure;

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int page);

    public Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page);
    public Task<Cheep> CreateCheep(string username, string email, string message, string timestamp, int id);
    public Task<Author> GetAuthorByName(string name);
    public Task<List<CheepDTO>> GetCheepsFromFollowedAuthor(IEnumerable<string> authors, int page);
    public Task<List<string>> GetFollowedAuthors(int authorId);
    public Task<bool> IsFollowing(int followingAuthorId, int followerAuthorId);
    public Task Unfollow(int followingAuthorId, int followerAuthorId);
    public Task CheckFollowerExistElseCreate(ApplicationUser user);
    public Task DeleteAuthorAndCheepsByEmail(string email);
    public Task<Bio> CreateBIO(string username, string email, string message, int id);
    public Task<List<BioDTO>> GetBio(int page);

}