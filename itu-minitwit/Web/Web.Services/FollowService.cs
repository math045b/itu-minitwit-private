namespace Web.Services;

public interface IFollowService
{
    public Task<bool> DoesFollow(string user, string potentialFollower);
    public Task Follow(string user, string follow);
    public Task UnFollow(string user, string unFollow);
}

public class FollowService : IFollowService
{
    public async Task<bool> DoesFollow(string user, string potentialFollower)
    {
        //TODO implement
        return new Random().Next(2) % 2 == 0;
    }

    public async Task Follow(string user, string follow)
    {
        //TODO implement
    }

    public async Task UnFollow(string user, string unFollow)
    {
        //TODO implement
    }
}