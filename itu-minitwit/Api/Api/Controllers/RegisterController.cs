using Api.CustomExceptions;
using Api.Services.Dto_s;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("[Controller]")]
[ApiController]
public class RegisterController(IUserService userService, ILatestService latestService)
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

        try
        {
            userService.Register(new CreateUserDTO(){Username = username, Email = email, Password = psw});
        }
        catch (UserAlreadyExists)
        {
            return new JsonResult(new { status = 400, error_msg = "The username is already taken" })
            {
                StatusCode = 400
            };
        }

        return Ok("");
    }
}