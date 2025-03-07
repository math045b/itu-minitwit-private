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
    public async Task<ActionResult> Register([FromBody] CreateUserDTO request, [FromQuery] int? latest)
    {
        await latestService.UpdateLatest(latest);

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return new JsonResult(new { status = 400, error_msg = "You have to enter a username" })
            {
                StatusCode = 400
            };
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
        {
            return new JsonResult(new { status = 400, error_msg = "You have to enter a valid email address" })
            {
                StatusCode = 400
            };
        }

        if (string.IsNullOrWhiteSpace(request.Pwd))
        {
            return new JsonResult(new { status = 400, error_msg = "You have to enter a password" })
            {
                StatusCode = 400
            };
        }

        try
        {
            userService.Register(new CreateUserDTO() { Username = request.Username, Email = request.Email, Pwd = request.Pwd });
        }
        catch (Exception e)
        {
            if (e.InnerException is UserAlreadyExists)
            {
                return new JsonResult(new { status = 400, error_msg = "The username is already taken" })
                {
                    StatusCode = 400
                };
            }
            return new JsonResult(new { status = 400, error_msg = e.Message })
            {
                StatusCode = 400
            };
        }

        return Ok("");
    }
}