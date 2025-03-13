using Api.Services.Dto_s.MessageDTO_s;
using Microsoft.AspNetCore.Mvc;
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
        try
        {
            logger.LogInformation($"Updating latest: {latest?.ToString() ?? "null"}");
            await latestService.UpdateLatest(latest);
            var messages = await db.ReadMessages();
            logger.LogInformation($"Message count: {messages.Count}");
            logger.LogInformation($"First message: {messages.First()}");
            logger.LogInformation($"Last message: {messages.Last()}");
            return Ok(messages);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured, that we did have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }
    }

    [LogMethodParameters]
    [IgnoreAntiforgeryToken]
    [HttpGet("msgs/{username}")]
    public async Task<IActionResult> GetFilteredMessages(string username, [FromQuery] int? latest)
    {
        try
        {
            logger.LogInformation($"Updating latest: {latest?.ToString() ?? "null"}");
            await latestService.UpdateLatest(latest);
            
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
            logger.LogError(e, "Couldn't find key");
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured, that we did have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }
    }

    [LogMethodParameters]
    [LogReturnValueAsync]
    [IgnoreAntiforgeryToken]
    [HttpPost("msgs/{username}")]
    public async Task<IActionResult> PostMessage(string username, [FromBody] CreateMessageDTO messageDto, [FromQuery] int? latest)
    {
        try
        {
            logger.LogInformation($"Updating latest: {latest?.ToString() ?? "null"}");
            await latestService.UpdateLatest(latest);
            
            await db.PostMessage(username, messageDto.Content);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            logger.LogError(e, "Couldn't find key");
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured, that we did have not accounted for");
            return StatusCode(500, "An error occured, that we did not for see");
        }
    }
}