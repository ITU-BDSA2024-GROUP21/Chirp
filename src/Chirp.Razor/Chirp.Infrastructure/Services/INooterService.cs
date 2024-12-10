namespace Chirp.Infrastructure;

public interface INooterService
{
    public Task<List<CheepDTO>> GetCheeps(int page);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int page);
    public Task<List<Cheep>> GetNootsWithoutPage(string author);
    public Task<Cheep> CreateCheep(string username, string email, string message, string timestamp, int id);
    public Task<Author> GetAuthorByName(string name);
    public Task DeleteNoot(int nootId);
    public Task<List<CheepDTO>> GetCheepsFromFollowedAuthor(IEnumerable<string> authors, int page);
    public Task<List<string>> GetFollowedAuthors(int authorId);
    public Task<bool> IsFollowing(int followingAuthorId, int followerAuthorId);
    public Task Follow(int followingAuthorId, int followerAuthorId);
    public Task Unfollow(int followingAuthorId, int followerAuthorId);
    public Task CheckFollowerExistElseCreate(ApplicationUser user);
    public Task DeleteAuthorAndCheepsByEmail(string email);
    public Task<Bio> CreateBio(string username, string email, string message, int id);
    public Task<BioDTO> GetBio(string author);
    public Task<bool> AuthorHasBio(string author);
    public Task DeleteBio(Author author);

}