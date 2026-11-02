
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAZOTECH.Trading.API.Data;

namespace SAZOTECH.Trading.API.Controllers;

[ApiController]
[Route("api/positions")]
public class PortfolioPositionsController : ControllerBase
{
    private readonly TradingDbContext _context;

    public PortfolioPositionsController(TradingDbContext context)
    {
        _context = context;
    }

    // GET: api/positions
    [HttpGet]
    public async Task<IActionResult> GetPositions()
    {
        var positions = await _context.PortfolioPositions
            .AsNoTracking()
            .Include(p => p.FinancialInstrument)
            .OrderBy(p => p.FinancialInstrument.Symbol)
            .Select(p => new
            {
                p.Id,
                p.FinancialInstrumentId,
                Symbol = p.FinancialInstrument.Symbol,
                Name = p.FinancialInstrument.Name,
                p.Quantity,
                p.AveragePrice,
                CurrentPrice = p.FinancialInstrument.LastPrice,
                MarketValue =
                    p.Quantity * p.FinancialInstrument.LastPrice,
                CostBasis =
                    p.Quantity * p.AveragePrice,
                UnrealizedPnL =
                    p.Quantity *
                    (p.FinancialInstrument.LastPrice - p.AveragePrice),
                p.UpdatedAt
            })
            .ToListAsync();

        return Ok(positions);
    }

    // GET: api/positions/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPositionById(int id)
    {
        var position = await _context.PortfolioPositions
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.FinancialInstrumentId,
                Symbol = p.FinancialInstrument.Symbol,
                Name = p.FinancialInstrument.Name,
                p.Quantity,
                p.AveragePrice,
                CurrentPrice = p.FinancialInstrument.LastPrice,
                MarketValue =
                    p.Quantity * p.FinancialInstrument.LastPrice,
                CostBasis =
                    p.Quantity * p.AveragePrice,
                UnrealizedPnL =
                    p.Quantity *
                    (p.FinancialInstrument.LastPrice - p.AveragePrice),
                p.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (position is null)
        {
            return NotFound(new
            {
                message = "Portfolio position not found."
            });
        }

        return Ok(position);
    }
}
