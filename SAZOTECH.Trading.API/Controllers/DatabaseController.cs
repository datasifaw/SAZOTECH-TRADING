
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAZOTECH.Trading.API.Data;

namespace SAZOTECH.Trading.API.Controllers;

[ApiController]
[Route("api/database")]
public class DatabaseController : ControllerBase
{
    private readonly TradingDbContext _context;

    public DatabaseController(TradingDbContext context)
    {
        _context = context;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        bool connected = await _context.Database.CanConnectAsync();

        if (!connected)
        {
            return StatusCode(503, new
            {
                Status = "Unavailable"
            });
        }

        return Ok(new
        {
            Status = "Connected",
            Database = "SazotechTradingDb"
        });
    }
}
