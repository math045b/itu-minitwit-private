using Microsoft.AspNetCore.Mvc;

namespace Api.Services.RepositoryInterfaces;

public interface IFollowRepository
{
    public Task<ActionResult> Follow();
    
    public Task<ActionResult> Unfollow();
    
    public Task<ActionResult> GetFollows();
}