using Api.DataAccess.Models;
using Api.DataAccess;
using Api.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("[Controller]")]
[ApiController]
public class RegisterController(MinitwitDbContext db, IPasswordHasher<User> passwordHasher, LatestService latestService)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Register([FromForm] string? username, [FromForm] string? email,
        [FromForm] string? psw, [FromQuery] int? latest)
    {
        await latestService.UpdateLatest(latest);

        if (string.IsNullOrWhiteSpace(username))
        {
            return new JsonResult(new { status = 400, error_msg = "You have to enter a username" })
            {
                StatusCode = 400
            };
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            return new JsonResult(new { status = 400, error_msg = "You have to enter a valid email address" })
            {
                StatusCode = 400
            };
        }

        if (string.IsNullOrWhiteSpace(psw))
        {
            return new JsonResult(new { status = 400, error_msg = "You have to enter a password" })
            {
                StatusCode = 400
            };
        }

        User user = new User
        {
            Username = username,
            Email = email,
        };
        user.PwHash = passwordHasher.HashPassword(user, psw);

        try
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            return new JsonResult(new { status = 400, error_msg = "The username is already taken" })
            {
                StatusCode = 400
            };
        }

        return Ok("");
    }
}