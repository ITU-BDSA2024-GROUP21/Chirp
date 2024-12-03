namespace Chirp.Infrastructure;

public interface IFollowRepository
{
    public Task FollowAuthor(int followingAuthorId, int followedAuthorId);
    public Task<List<string>> GetFollowedAuthors(int authorId);
    public Task<bool> IsFollowing(int followerId, int followedId);
    public Task Unfollow(int followingAuthorId, int followedAuthorId);
    
}