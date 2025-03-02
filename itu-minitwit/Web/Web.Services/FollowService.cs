namespace Web.Services;

public interface IFollowService {
    public Task Follow(string user, string follow);
    public Task UnFollow(string user, string unFollow);
}

public class FollowService : IFollowService
{
    public async Task Follow(string user, string follow)
    {
        //TODO implement
    }

    public async Task UnFollow(string user, string unFollow)
    {
        //TODO implement
    }
}