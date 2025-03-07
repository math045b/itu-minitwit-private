using Api.Services;
using Api.Services.CustomExceptions;
using Api.Services.Dto_s;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("[Controller]")]
[ApiController]
public class RegisterController(IUserService userService, ILatestService latestService, ILogger<RegisterController> logger)
    : ControllerBase
{
    [LogMethodParameters]
    [LogReturnValueAsync]
    [HttpPost]
    public async Task<ActionResult> Register([FromForm] string? username, [FromForm] string? email,
        [FromForm] string? psw, [FromQuery] int? latest)
    {
        try
        {
            logger.LogInformation($"Updating latest: {latest?.ToString() ?? "null"}");
            await latestService.UpdateLatest(latest);

            if (string.IsNullOrWhiteSpace(username))
            {
                logger.LogError($"Invalid username: {username}");
                return new JsonResult(new { status = 400, error_msg = "You have to enter a username" })
                {
                    StatusCode = 400
                };
            }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            {
                logger.LogError($"Invalid email: {email}");
                return new JsonResult(new { status = 400, error_msg = "You have to enter a valid email address" })
                {
                    StatusCode = 400
                };
            }

            if (string.IsNullOrWhiteSpace(psw))
            {
                logger.LogError($"Invalid password {psw}");
                return new JsonResult(new { status = 400, error_msg = "You have to enter a password" })
                {
                    StatusCode = 400
                };
            }

            try
            {
               await userService.Register(new CreateUserDTO() { Username = username, Email = email, Password = psw });
            }
            catch (UserAlreadyExists e)
            {
                logger.LogError(e, $"User {username} is already registered");
                return new JsonResult(new { status = 400, error_msg = "The username is already taken" })
                {
                    StatusCode = 400
                };
            }

            return Ok("");
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured, that we did have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }
    }
}