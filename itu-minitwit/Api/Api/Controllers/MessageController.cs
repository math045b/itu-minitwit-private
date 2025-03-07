using Api.Services;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("/")]
public class MessageController(IMessageService db, ILatestService latestService, ILogger<MessageController> logger) : Controller
{

    [LogMethodParameters]
    [IgnoreAntiforgeryToken]
    [HttpGet("msgs")]
    public async Task<IActionResult> GetMessages([FromQuery] int? latest)
    {
        logger.LogInformation($"Updating latest: {latest?.ToString() ?? "null"}");
        await latestService.UpdateLatest(latest);
        var messages = db.ReadMessages().Result;
        logger.LogInformation($"Message count: {messages.Count}");
        logger.LogInformation($"First message: {messages.First()}");
        logger.LogInformation($"Last message: {messages.Last()}");
        return Ok(messages);
    }

    [LogMethodParameters]
    [IgnoreAntiforgeryToken]
    [HttpGet("msgs/{username}")]
    public async Task<IActionResult> GetFilteredMessages(string username)
    {
        await latestService.UpdateLatest(1);
        try
        {
            var filteredMessages = await db.ReadFilteredMessages(username, 100);
            if (filteredMessages.Count == 0)
            {
                logger.LogInformation("Didn't find any messages");
                return NoContent();
            }
            logger.LogInformation($"Message count: {filteredMessages.Count}");
            logger.LogInformation($"First message: {filteredMessages.First()}");
            logger.LogInformation($"Last message: {filteredMessages.Last()}");
            return Ok(filteredMessages);
        }
        catch (KeyNotFoundException e)
        {
            logger.LogError(e, "ERROR: Couldn't find key");
            return NotFound(new { message = e.Message });
        }
    }

    [LogMethodParameters]
    [LogReturnValueAsync]
    [IgnoreAntiforgeryToken]
    [HttpPost("msgs/{username}")]
    public async Task<IActionResult> PostMessage(string username, [FromForm] string content)
    {
        try
        {
            await db.PostMessage(username, content);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            logger.LogError(e, "ERROR: Couldn't find key");
            return NotFound(new { message = e.Message });
        }
    }
}