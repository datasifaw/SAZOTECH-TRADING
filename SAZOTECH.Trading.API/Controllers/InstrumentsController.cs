
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAZOTECH.Trading.API.Data;
using SAZOTECH.Trading.API.Models;

namespace SAZOTECH.Trading.API.Controllers;

[ApiController]
[Route("api/instruments")]
public class InstrumentsController : ControllerBase
{
    private readonly TradingDbContext _context;

    public InstrumentsController(TradingDbContext context)
    {
        _context = context;
    }

    // GET: api/instruments
    [HttpGet]
    public async Task<ActionResult<List<FinancialInstrument>>> GetAll()
    {
        var instruments = await _context.FinancialInstruments
            .AsNoTracking()
            .OrderBy(i => i.Symbol)
            .ToListAsync();

        return Ok(instruments);
    }

    // POST: api/instruments
    [HttpPost]
    public async Task<ActionResult<FinancialInstrument>> Create(
        [FromBody] FinancialInstrument instrument)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(instrument.Symbol) ||
            string.IsNullOrWhiteSpace(instrument.Name) ||
            string.IsNullOrWhiteSpace(instrument.Exchange) ||
            string.IsNullOrWhiteSpace(instrument.Currency) ||
            instrument.LastPrice <= 0)
        {
            return BadRequest(new
            {
                Message = "Invalid financial instrument data."
            });
        }

        // Normalize the symbol
        instrument.Symbol = instrument.Symbol.Trim().ToUpperInvariant();

        // Check for duplicates
        bool exists = await _context.FinancialInstruments
            .AnyAsync(i => i.Symbol == instrument.Symbol);

        if (exists)
        {
            return Conflict(new
            {
                Message = "An instrument with this symbol already exists."
            });
        }

        // Set server-managed fields
        instrument.Id = 0;
        instrument.CreatedAt = DateTime.UtcNow;

        // Save to SQL Server
        _context.FinancialInstruments.Add(instrument);
        await _context.SaveChangesAsync();

        return StatusCode(201, instrument);
    }
}
