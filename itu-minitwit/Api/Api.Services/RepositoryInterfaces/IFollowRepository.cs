using Microsoft.AspNetCore.Mvc;

namespace Api.Services.RepositoryInterfaces;

public interface IFollowRepository
{
    public Task Follow(string username, string follow);
    
    public Task Unfollow(string username, string follow);
    
    public Task<IEnumerable<string>> GetFollows(string username, [FromQuery] int? latest, [FromQuery] int no = 100);
}