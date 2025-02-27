using Api.DataAccess.Models;
using Api.Services.Exceptions;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("[Controller]")]
[ApiController]
public class FollowerController(IFollowService followService, ILatestService latestService) : ControllerBase
{
    [HttpPost("/fllws/{username}")]
    public async Task<ActionResult> FollowOrUnfollow(string username, [FromForm] string? follow, [FromForm] string? unfollow, [FromQuery] int? latest)
    {
        await latestService.UpdateLatest(latest);
        
        if (follow != null)
        {
            if (username == follow)
            {
                return BadRequest("You cannot follow yourself");
            }
            
            return await Follow(username, follow);
        }
        if (unfollow != null)
        {
            if (username == unfollow)
            {
                return BadRequest("You cannot follow yourself");
            }
            
            return await Unfollow(username, unfollow);
        }

        return BadRequest("You must provide a user to follow or unfollow");
    }
    
    private async Task<ActionResult> Follow(string username, string follow)
    {
        try
        {
            await followService.FollowUser(username, follow);
        }
        catch (UserDoesntExistException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (AlreadyFollowsUserException e)
        {
            Console.WriteLine(e.Message);
        }

        return NoContent();
    }
    
    private async Task<ActionResult> Unfollow(string username, string unfollow)
    {
        try
        {
            await followService.UnfollowUser(username, unfollow);
        }
        catch (UserDoesntExistException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (AlreadyFollowsUserException e)
        {
            Console.WriteLine(e.Message);
        }

        return NoContent();
    }

    [HttpGet("/fllws/{username}")]
    public async Task<IActionResult> GetFollows(string username, [FromQuery] int? latest, [FromQuery] int no = 100)
    {
        try
        {
            await latestService.UpdateLatest(latest);

            var follows = await followService.GetFollows(username, no);
            return Ok(new { follows });
        }
        catch (UserDoesntExistException e)
        {
            return NotFound(new { message = e.Message });
        }
    }

}