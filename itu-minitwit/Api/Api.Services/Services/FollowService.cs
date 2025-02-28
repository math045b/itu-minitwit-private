using Api.Services.RepositoryInterfaces;

namespace Api.Services.Services;

public interface IFollowService
{
    public Task FollowUser(string username, string follow);
    public Task UnfollowUser(string username, string unfollow);
    public IEnumerable<string> GetFollows(string username, int no);
}

public class FollowService(IFollowRepository followRepository) : IFollowService
{
    public Task FollowUser(string username, string follow)
    {
        return followRepository.Follow(username, follow);
    }

    public Task UnfollowUser(string username, string unfollow)
    {
        return followRepository.Unfollow(username, unfollow);
    }

    public IEnumerable<string> GetFollows(string username, int no)
    {
        return followRepository.GetFollows(username, no).Result;
    }
}