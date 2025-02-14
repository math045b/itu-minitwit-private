using Microsoft.AspNetCore.Mvc;

namespace itu_minitwit.SimulatorAPI;

[Route("api[Controller]")]
[ApiController]
public class LatestController(LatestService latestService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetLatest()
    {
        var latestProcessedCommandId = await latestService.GetLatest();

        return Ok(new { latest = latestProcessedCommandId });
    }
}