using System.Net;
using itu_minitwit.Data;
using Microsoft.AspNetCore.Mvc;

namespace itu_minitwit.SimulatorAPI;


[Route("[Controller]")]
[ApiController]
public class FollowerController(MiniTwitDbContext dbContext, LatestService latestService) : ControllerBase
{
    [HttpPost("/fllws/{username}")]
    public async Task<ActionResult> Follow(string username, [FromForm] string? follow)
    {
        await latestService.UpdateLatest(-1); //TODO: should change the update number
        
        var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
        var userToFollow = dbContext.Users.FirstOrDefault(u => u.Username == follow);

        if (user == null || userToFollow == null)
        {
            return NotFound();
        }
        
        var followRelation = dbContext.Followers.FirstOrDefault(f => f.WhoId == user!.UserId && 
                                                                     f.WhomId == userToFollow!.UserId);

        followRelation = new Follower
        {
            WhoId = user.UserId,
            WhomId = userToFollow.UserId
        };

        dbContext.Followers.Add(followRelation);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
    
    private async Task<ActionResult> Unfollow(string username, string unfollow)
    {
        var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
        var userToUnfollow = dbContext.Users.FirstOrDefault(u => u.Username == unfollow);
    
        if (user == null || userToUnfollow == null)
        {
            return NotFound();
        }
    
        var followRelation =
            dbContext.Followers.FirstOrDefault(f => f.WhoId == user!.UserId
                                                    && f.WhomId == userToUnfollow!.UserId);
    
        if (followRelation == null) return NotFound();
    
        dbContext.Followers.Remove(followRelation);
        await dbContext.SaveChangesAsync();
    
        return NoContent();
    }
}