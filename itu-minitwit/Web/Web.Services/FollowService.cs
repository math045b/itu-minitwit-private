namespace Web.Services;

public interface IFollowService
{
    public Task<bool> DoesFollow(string user, string potentialFollower);
    public Task Follow(string user, string follow);
    public Task UnFollow(string user, string unFollow);
}

public class FollowService : IFollowService
{
    public Task<bool> DoesFollow(string user, string potentialFollower)
    {
        //TODO implement
        return Task.FromResult(new Random().Next(2) % 2 == 0);
    }

    public Task Follow(string user, string follow)
    {
        return Task.CompletedTask;
        //TODO implement
    }

    public Task UnFollow(string user, string unFollow)
    {
        return Task.CompletedTask;
        //TODO implement
    }
}