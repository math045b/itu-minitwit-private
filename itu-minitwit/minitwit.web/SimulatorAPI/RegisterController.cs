using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace itu_minitwit.SimulatorAPI;

[Route("[Controller]")]
[ApiController]
public class RegisterController(LatestService latestService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Register([FromForm] string? username, [FromForm] string? email, [FromForm] string? psw)
    {
        await latestService.UpdateLatest(-1);

        
        if (string.IsNullOrWhiteSpace(username))
        {
            return new JsonResult(new { status = 400, error_msg = "You have to enter a username"})
            {
                StatusCode = 400
            };
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return new JsonResult(new { status = 400, error_msg = "You have to enter a valid email address"})
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

        return Ok("");
    }
}