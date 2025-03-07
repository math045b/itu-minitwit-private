using System.Runtime.InteropServices.JavaScript;
using Api.DataAccess.Models;
using Api.Services;
using Api.Services.CustomExceptions;
using Api.Services.Exceptions;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Api.Controllers;

[Route("[Controller]")]
[ApiController]
public class FollowerController(IFollowService followService, ILatestService latestService, ILogger<FollowerController> logger) : ControllerBase
{
    [LogMethodParameters]
    [LogReturnValueAsync]
    [HttpPost("/fllws/{username}")]
    public async Task<ActionResult> FollowOrUnfollow(string username, [FromForm] string? follow,
        [FromForm] string? unfollow, [FromQuery] int? latest)
    {
        try
        {
            logger.LogInformation($"Updating latest: {latest?.ToString() ?? "null"}");
            await latestService.UpdateLatest(latest);

            if (follow != null)
            {
                if (username == follow)
                {
                    logger.LogError("You cannot follow yourself");
                    return BadRequest("You cannot follow yourself");
                }

                return await Follow(username, follow);
            }

            if (unfollow != null)
            {
                if (username == unfollow)
                {
                    logger.LogError("You cannot follow yourself");
                    return BadRequest("You cannot follow yourself");
                }

                return await Unfollow(username, unfollow);
            }

            logger.LogError("You must provide a user to follow or unfollow");
            return BadRequest("You must provide a user to follow or unfollow");
        } 
        catch (Exception e)
        {
            logger.LogError(e, "An error occured, that we did have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }
    }

    [LogMethodParameters]
    [LogReturnValueAsync]
    private async Task<ActionResult> Follow(string username, string follow)
    {
        try
        {
            await followService.FollowUser(username, follow);
        }
        catch (UserDoesntExistException e)
        {
            logger.LogError(e, "User doesn't exists");
            return NotFound(e.Message);
        }
        catch (AlreadyFollowsUserException e)
        {
            logger.LogError(e, $"\"{username}\" already follows \"{follow}\"");
            return BadRequest(e.Message);
        }

        return NoContent();
    }
    
    [LogMethodParameters]
    [LogReturnValueAsync]
    private async Task<ActionResult> Unfollow(string username, string unfollow)
    {
        try
        {
            await followService.UnfollowUser(username, unfollow);
        }
        catch (UserDoesntExistException e)
        {
            logger.LogError(e, "User doesn't exists");
            return NotFound(e.Message);
        }
        catch (DontFollowUserException e)
        {
            logger.LogError(e, $"\"{username}\" already follows \"{unfollow}\"");
            return BadRequest(e.Message);
        }

        return NoContent();
    }

    [LogMethodParameters]
    [LogReturnValueAsync]
    [HttpGet("/fllws/{username}")]
    public async Task<IActionResult> GetFollows(string username, [FromQuery] int? latest, [FromQuery] int no = 100)
    {
        try
        {
            logger.LogInformation($"Updating latest: {latest?.ToString() ?? "null"}");
            await latestService.UpdateLatest(latest);

            var follows = followService.GetFollows(username, no);
            return Ok(new { follows });
        }
        catch (UserDoesntExistException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured, that we did have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }
    }

}