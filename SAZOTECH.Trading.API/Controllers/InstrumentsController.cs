
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

    // GET: api/instruments/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FinancialInstrument>> GetById(int id)
    {
        var instrument = await _context.FinancialInstruments
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        if (instrument is null)
        {
            return NotFound(new
            {
                Message = $"Instrument with ID {id} was not found."
            });
        }

        return Ok(instrument);
    }

    // POST: api/instruments
    [HttpPost]
    public async Task<ActionResult<FinancialInstrument>> Create(
        [FromBody] FinancialInstrument instrument)
    {
        Normalize(instrument);

        if (!IsValid(instrument))
        {
            return BadRequest(new
            {
                Message = "Invalid instrument data. Check required fields, lengths, currency code and price."
            });
        }

        bool exists = await _context.FinancialInstruments
            .AnyAsync(i => i.Symbol == instrument.Symbol);

        if (exists)
        {
            return Conflict(new
            {
                Message = "An instrument with this symbol already exists."
            });
        }

        instrument.Id = 0;
        instrument.CreatedAt = DateTime.UtcNow;

        _context.FinancialInstruments.Add(instrument);
        await _context.SaveChangesAsync();

        return StatusCode(201, instrument);
    }

    // PUT: api/instruments/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] FinancialInstrument updatedInstrument)
    {
        var instrument = await _context.FinancialInstruments
            .FirstOrDefaultAsync(i => i.Id == id);

        if (instrument is null)
        {
            return NotFound(new
            {
                Message = $"Instrument with ID {id} was not found."
            });
        }

        Normalize(updatedInstrument);

        if (!IsValid(updatedInstrument))
        {
            return BadRequest(new
            {
                Message = "Invalid instrument data. Check required fields, lengths, currency code and price."
            });
        }

        bool duplicateSymbol = await _context.FinancialInstruments
            .AnyAsync(i =>
                i.Symbol == updatedInstrument.Symbol &&
                i.Id != id);

        if (duplicateSymbol)
        {
            return Conflict(new
            {
                Message = "An instrument with this symbol already exists."
            });
        }

        instrument.Symbol = updatedInstrument.Symbol;
        instrument.Name = updatedInstrument.Name;
        instrument.Exchange = updatedInstrument.Exchange;
        instrument.LastPrice = updatedInstrument.LastPrice;
        instrument.Currency = updatedInstrument.Currency;

        await _context.SaveChangesAsync();

        return Ok(instrument);
    }

    // DELETE: api/instruments/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var instrument = await _context.FinancialInstruments
            .FirstOrDefaultAsync(i => i.Id == id);

        if (instrument is null)
        {
            return NotFound(new
            {
                Message = $"Instrument with ID {id} was not found."
            });
        }

        _context.FinancialInstruments.Remove(instrument);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Normalize financial instrument data
    private static void Normalize(FinancialInstrument instrument)
    {
        instrument.Symbol =
            instrument.Symbol?.Trim().ToUpperInvariant() ?? "";

        instrument.Name =
            instrument.Name?.Trim() ?? "";

        instrument.Exchange =
            instrument.Exchange?.Trim() ?? "";

        instrument.Currency =
            instrument.Currency?.Trim().ToUpperInvariant() ?? "";
    }

    // Validate against SQL Server constraints
    private static bool IsValid(FinancialInstrument instrument)
    {
        return !string.IsNullOrWhiteSpace(instrument.Symbol)
            && instrument.Symbol.Length <= 20
            && !string.IsNullOrWhiteSpace(instrument.Name)
            && instrument.Name.Length <= 200
            && !string.IsNullOrWhiteSpace(instrument.Exchange)
            && instrument.Exchange.Length <= 50
            && instrument.Currency.Length == 3
            && instrument.Currency.All(c => c >= 'A' && c <= 'Z')
            && instrument.LastPrice > 0
            && instrument.LastPrice <= 99999999999999.9999m
            && decimal.Round(instrument.LastPrice, 4)
                == instrument.LastPrice;
    }
}
