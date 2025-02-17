using System.Net;
using itu_minitwit.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace itu_minitwit.SimulatorAPI;


[Route("[Controller]")]
[ApiController]
public class FollowerController(MiniTwitDbContext dbContext, LatestService latestService) : ControllerBase
{
    [HttpPost("/fllws/{username}")]
    public async Task<ActionResult> FollowOrUnfollow(string username, [FromForm] string? follow, [FromForm] string? unfollow)
    {
        await latestService.UpdateLatest(-1);
        
        if (follow != null)
        {
            return await Follow(username, follow);
        }
        if (unfollow != null)
        {
            return await Unfollow(username, unfollow);
        }

        return BadRequest("You must provide a user to follow or unfollow");
    }
    
    private async Task<ActionResult> Follow(string username, string follow)
    {
        var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
        var userToFollow = dbContext.Users.FirstOrDefault(u => u.Username == follow);
    
        if (user == null || userToFollow == null)
        {
            return NotFound();
        }
    
        var followRelation = dbContext.Followers.FirstOrDefault(f => f.WhoId == user!.UserId
                                                                    && f.WhomId == userToFollow!.UserId);
    
        if (followRelation != null) return BadRequest("You already follow that user");
    
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

    [HttpGet("/fllws/{username}")]
    public async Task<ActionResult> GetFollows(string username, [FromBody] int no = 100)
    {
        await latestService.UpdateLatest(-1); //TODO: change the update number
    
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    
    
        var following = await dbContext.Followers
            .Where(f => f.WhoId == user!.UserId)
            .Join(dbContext.Users,
                f => f.WhomId,
                u => u.UserId,
                (f, u) => new { user = u.Username, follows = f.WhoId }
            )
            .Take(no)
            .ToListAsync();
        
        return Ok(following);
    }
}