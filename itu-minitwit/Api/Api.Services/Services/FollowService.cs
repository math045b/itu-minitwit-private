using Api.Services.RepositoryInterfaces;

namespace Api.Services.Services;

public interface IFollowService
{
    public Task FollowUser(string username, string follow);
    public Task UnfollowUser(string username, string unfollow);
    public Task<IEnumerable<string>> GetFollows(string username, int no);
}

public class FollowService(IFollowRepository followRepository) : IFollowService
{
    public Task FollowUser(string username, string follow)
    {
        return Task.FromResult(followRepository.Follow(username, follow));
    }

    public Task UnfollowUser(string username, string unfollow)
    {
        return Task.FromResult(followRepository.Unfollow(username, unfollow));
    }

    public Task<IEnumerable<string>> GetFollows(string username, int no)
    {
        return Task.FromResult(followRepository.GetFollows(username, no).Result);
    }
}