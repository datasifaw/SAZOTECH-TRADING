
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAZOTECH.Trading.API.Data;
using SAZOTECH.Trading.API.Models;

namespace SAZOTECH.Trading.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderExecutionsController : ControllerBase
{
    private readonly TradingDbContext _context;

    public OrderExecutionsController(TradingDbContext context)
    {
        _context = context;
    }

    [HttpPost("{id:int}/execute")]
    public async Task<IActionResult> ExecuteOrder(int id)
    {
        // Keep order execution and portfolio updates atomic.
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var order = await _context.TradingOrders
            .Include(o => o.FinancialInstrument)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            return NotFound(new
            {
                message = "Trading order not found."
            });
        }

        if (order.Status != "NEW")
        {
            return Conflict(new
            {
                message = "Only NEW orders can be executed."
            });
        }

        if (order.Quantity <= 0 ||
            order.Price <= 0 ||
            (order.OrderType != "BUY" &&
             order.OrderType != "SELL"))
        {
            return BadRequest(new
            {
                message = "The trading order contains invalid data."
            });
        }

        var position = await _context.PortfolioPositions
            .FirstOrDefaultAsync(p =>
                p.FinancialInstrumentId ==
                order.FinancialInstrumentId);

        if (order.OrderType == "BUY")
        {
            if (position is null)
            {
                position = new PortfolioPosition
                {
                    FinancialInstrumentId =
                        order.FinancialInstrumentId,
                    Quantity = order.Quantity,
                    AveragePrice = order.Price,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.PortfolioPositions.Add(position);
            }
            else
            {
                // Calculate the weighted average acquisition price.
                var existingCost =
                    position.Quantity * position.AveragePrice;

                var newCost =
                    order.Quantity * order.Price;

                var newQuantity = checked(
                    position.Quantity + order.Quantity);

                position.AveragePrice =
                    (existingCost + newCost) / newQuantity;

                position.Quantity = newQuantity;
                position.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            // SELL orders require sufficient available holdings.
            if (position is null ||
                position.Quantity < order.Quantity)
            {
                return Conflict(new
                {
                    message =
                        "Insufficient portfolio quantity to execute SELL order."
                });
            }

            position.Quantity -= order.Quantity;
            position.UpdatedAt = DateTime.UtcNow;

            // Remove fully closed positions.
            if (position.Quantity == 0)
            {
                _context.PortfolioPositions.Remove(position);
            }
        }

        order.Status = "EXECUTED";

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(new
        {
            message = "Trading order executed successfully.",
            orderId = order.Id,
            symbol = order.FinancialInstrument.Symbol,
            orderType = order.OrderType,
            executedQuantity = order.Quantity,
            executionPrice = order.Price,
            totalAmount = order.TotalAmount,
            status = order.Status,
            remainingQuantity =
                position is not null &&
                _context.Entry(position).State != EntityState.Deleted
                    ? position.Quantity
                    : 0,
            averagePrice =
                position is not null &&
                _context.Entry(position).State != EntityState.Deleted
                    ? position.AveragePrice
                    : 0m
        });
    }
}
