using Microsoft.AspNetCore.Mvc;

namespace itu_minitwit.SimulatorAPI;

[Route("[Controller]")]
[ApiController]
public class RegisterController(LatestService latestService) : ControllerBase
{
}