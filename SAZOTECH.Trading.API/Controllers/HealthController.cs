
using Microsoft.AspNetCore.Mvc;

namespace SAZOTECH.Trading.API.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            Status = "Healthy",
            Application = "SAZOTECH-TRADING",
            Timestamp = DateTime.UtcNow
        });
    }
}
