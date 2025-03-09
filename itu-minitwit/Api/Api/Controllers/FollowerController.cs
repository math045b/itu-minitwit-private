using Api.DataAccess.Models;
using Api.Services.Dto_s.FollowDTO_s;
using Api.Services.Exceptions;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("[Controller]")]
[ApiController]
public class FollowerController(IFollowService followService, ILatestService latestService) : ControllerBase
{
    [HttpPost("/fllws/{username}")]
    public async Task<ActionResult> FollowOrUnfollow(string username, [FromBody] FollowDTO followDto, [FromQuery] int? latest)
    {
        await latestService.UpdateLatest(latest);
        
        if (followDto.Follow != null)
        {
            if (username == followDto.Follow)
            {
                return BadRequest("You cannot follow yourself");
            }
            
            return await Follow(username, followDto.Follow);
        }
        if (followDto.Unfollow != null)
        {
            if (username == followDto.Unfollow)
            {
                return BadRequest("You cannot follow yourself");
            }
            
            return await Unfollow(username, followDto.Unfollow);
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
            return NotFound(e.Message);
        }
        catch (AlreadyFollowsUserException e)
        {
            return BadRequest(e.Message);
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
            return NotFound(e.Message);
        }
        catch (AlreadyFollowsUserException e)
        {
            return BadRequest(e.Message);
        }

        return NoContent();
    }

    [HttpGet("/fllws/{username}")]
    public async Task<IActionResult> GetFollows(string username, [FromQuery] int? latest, [FromQuery] int no = 100)
    {
        try
        {
            await latestService.UpdateLatest(latest);

            var follows = followService.GetFollows(username, no);
            return Ok(new { follows });
        }
        catch (UserDoesntExistException e)
        {
            return NotFound(new { message = e.Message });
        }
    }

}