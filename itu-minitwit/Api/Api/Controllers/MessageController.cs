using Microsoft.AspNetCore.Mvc;
using Api.Services.Services;
using Newtonsoft.Json;

namespace itu_minitwit.Controllers;

[Route("/")]
public class MessageController(IMessageService db, ILatestService latestService) : Controller
{

    [IgnoreAntiforgeryToken]
    [HttpGet("msgs")]
    public async Task<IActionResult> GetMessages([FromQuery] int? latest)
    {
        await latestService.UpdateLatest(latest);

        var messages = db.ReadMessages();

        return Json(messages);
    }


    [IgnoreAntiforgeryToken]
    [HttpGet("msgs/{username}")]
    public async Task<IActionResult> GetFilteredMessages(string username)
    {
        await latestService.UpdateLatest(1);
        try
        {
            var filtered_messages = await db.ReadFilteredMessages(username, 100);
            if (filtered_messages.Count == 0)
            {
                return NoContent();

            }
            return Ok(filtered_messages);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
    }


    [IgnoreAntiforgeryToken]
    [HttpPost("msgs/{username}")]
    public async Task<IActionResult> PostMessage(string username, [FromForm] string content)
    {
        
        try
        {
            await db.PostMessage(username, content);
            TempData["FlashMessages"] = JsonConvert.SerializeObject(new List<string> { "Your message was recorded." });
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        
        
    }
    
    
}