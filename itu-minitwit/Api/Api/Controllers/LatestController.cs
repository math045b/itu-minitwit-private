using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class LatestController(ILatestService latestService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetLatest()
    {
        var latestProcessedCommandId = await latestService.GetLatest();

        return Ok(new { latest = latestProcessedCommandId });
    }
}