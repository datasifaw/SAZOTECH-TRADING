
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAZOTECH.Trading.API.Data;
using SAZOTECH.Trading.API.Models;

namespace SAZOTECH.Trading.API.Controllers;

[ApiController]
[Route("api/orders")]
public class TradingOrdersController : ControllerBase
{
    private readonly TradingDbContext _context;

    public TradingOrdersController(TradingDbContext context)
    {
        _context = context;
    }

    // GET: api/orders
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.TradingOrders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .ThenByDescending(o => o.Id)
            .Select(o => new
            {
                o.Id,
                o.FinancialInstrumentId,
                Symbol = o.FinancialInstrument.Symbol,
                o.OrderType,
                o.Quantity,
                o.Price,
                o.TotalAmount,
                o.Status,
                o.CreatedAt
            })
            .ToListAsync();

        return Ok(orders);
    }

    // GET: api/orders/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _context.TradingOrders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new
            {
                o.Id,
                o.FinancialInstrumentId,
                Symbol = o.FinancialInstrument.Symbol,
                o.OrderType,
                o.Quantity,
                o.Price,
                o.TotalAmount,
                o.Status,
                o.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (order is null)
        {
            return NotFound(new
            {
                Message = $"Trading order with ID {id} was not found."
            });
        }

        return Ok(order);
    }

    // POST: api/orders
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTradingOrderRequest request)
    {
        string orderType = request.OrderType?
            .Trim()
            .ToUpperInvariant() ?? "";

        if (orderType != "BUY" && orderType != "SELL")
        {
            return BadRequest(new
            {
                Message = "Order type must be BUY or SELL."
            });
        }

        if (request.Quantity <= 0)
        {
            return BadRequest(new
            {
                Message = "Quantity must be greater than zero."
            });
        }

        var instrument = await _context.FinancialInstruments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                i => i.Id == request.FinancialInstrumentId);

        if (instrument is null)
        {
            return NotFound(new
            {
                Message = "Financial instrument not found."
            });
        }

        decimal price = instrument.LastPrice;
        decimal totalAmount;

        try
        {
            totalAmount = checked(price * request.Quantity);
        }
        catch (OverflowException)
        {
            return BadRequest(new
            {
                Message = "Order amount is too large."
            });
        }

        if (price <= 0 ||
            totalAmount > 99999999999999.9999m)
        {
            return BadRequest(new
            {
                Message = "Invalid order price or total amount."
            });
        }

        var order = new TradingOrder
        {
            FinancialInstrumentId = instrument.Id,
            OrderType = orderType,
            Quantity = request.Quantity,
            Price = price,
            TotalAmount = totalAmount,
            Status = "NEW",
            CreatedAt = DateTime.UtcNow
        };

        _context.TradingOrders.Add(order);
        await _context.SaveChangesAsync();

        return StatusCode(201, new
        {
            order.Id,
            order.FinancialInstrumentId,
            instrument.Symbol,
            order.OrderType,
            order.Quantity,
            order.Price,
            order.TotalAmount,
            order.Status,
            order.CreatedAt
        });
    }
}

// Request DTO
public class CreateTradingOrderRequest
{
    public int FinancialInstrumentId { get; set; }

    public string OrderType { get; set; } = string.Empty;

    public int Quantity { get; set; }
}
