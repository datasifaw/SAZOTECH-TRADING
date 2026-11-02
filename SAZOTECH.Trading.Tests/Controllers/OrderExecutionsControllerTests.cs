
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SAZOTECH.Trading.API.Controllers;
using SAZOTECH.Trading.API.Data;
using SAZOTECH.Trading.API.Models;
using Xunit;

namespace SAZOTECH.Trading.Tests.Controllers;

public class OrderExecutionsControllerTests
{
    private static async Task<TradingDbContext> CreateContextAsync(
        SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<TradingDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new TradingDbContext(options);

        await context.Database.EnsureCreatedAsync();

        return context;
    }

    private static async Task<FinancialInstrument> CreateInstrumentAsync(
        TradingDbContext context)
    {
        var instrument = new FinancialInstrument
        {
            Symbol = "AAPL",
            Name = "Apple Inc.",
            Exchange = "NASDAQ",
            Currency = "USD",
            LastPrice = 225.75m
        };

        context.FinancialInstruments.Add(instrument);
        await context.SaveChangesAsync();

        return instrument;
    }

    private static async Task<TradingOrder> CreateOrderAsync(
        TradingDbContext context,
        int instrumentId,
        string orderType,
        int quantity,
        decimal price,
        string status = "NEW")
    {
        var order = new TradingOrder
        {
            FinancialInstrumentId = instrumentId,
            OrderType = orderType,
            Quantity = quantity,
            Price = price,
            TotalAmount = quantity * price,
            Status = status
        };

        context.TradingOrders.Add(order);
        await context.SaveChangesAsync();

        return order;
    }

    private static async Task CreatePositionAsync(
        TradingDbContext context,
        int instrumentId,
        int quantity,
        decimal averagePrice)
    {
        var position = new PortfolioPosition
        {
            FinancialInstrumentId = instrumentId,
            Quantity = quantity,
            AveragePrice = averagePrice
        };

        context.PortfolioPositions.Add(position);
        await context.SaveChangesAsync();
    }

    // TEST 1: Order not found -> HTTP 404

    [Fact]
    public async Task ExecuteOrder_WhenOrderDoesNotExist_ReturnsNotFound()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context =
            await CreateContextAsync(connection);

        var controller = new OrderExecutionsController(context);

        var result = await controller.ExecuteOrder(999);

        Assert.IsType<NotFoundObjectResult>(result);

        Assert.Empty(
            await context.PortfolioPositions.ToListAsync());
    }

    // TEST 2: Already executed BUY -> HTTP 409

    [Fact]
    public async Task ExecuteOrder_WhenAlreadyExecuted_ReturnsConflict()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context =
            await CreateContextAsync(connection);

        var instrument = await CreateInstrumentAsync(context);

        var order = await CreateOrderAsync(
            context,
            instrument.Id,
            "BUY",
            10,
            100m,
            "EXECUTED");

        var controller = new OrderExecutionsController(context);

        var result = await controller.ExecuteOrder(order.Id);

        Assert.IsType<ConflictObjectResult>(result);

        context.ChangeTracker.Clear();

        var savedOrder = await context.TradingOrders
            .AsNoTracking()
            .SingleAsync(o => o.Id == order.Id);

        Assert.Equal("EXECUTED", savedOrder.Status);

        Assert.Empty(
            await context.PortfolioPositions.ToListAsync());
    }

    // TEST 3: Initial BUY creates a position

    [Fact]
    public async Task ExecuteOrder_WhenBuyIsNew_CreatesPortfolioPosition()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context =
            await CreateContextAsync(connection);

        var instrument = await CreateInstrumentAsync(context);

        var order = await CreateOrderAsync(
            context,
            instrument.Id,
            "BUY",
            10,
            225.75m);

        var controller = new OrderExecutionsController(context);

        var result = await controller.ExecuteOrder(order.Id);

        Assert.IsType<OkObjectResult>(result);

        context.ChangeTracker.Clear();

        var savedOrder = await context.TradingOrders
            .AsNoTracking()
            .SingleAsync(o => o.Id == order.Id);

        var position = await context.PortfolioPositions
            .AsNoTracking()
            .SingleAsync(p =>
                p.FinancialInstrumentId == instrument.Id);

        Assert.Equal("EXECUTED", savedOrder.Status);
        Assert.Equal(10, position.Quantity);
        Assert.Equal(225.75m, position.AveragePrice);
    }

    // TEST 4: Additional BUY updates weighted average price

    [Fact]
    public async Task ExecuteOrder_WhenBuyingExistingPosition_UpdatesWeightedAveragePrice()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context =
            await CreateContextAsync(connection);

        var instrument = await CreateInstrumentAsync(context);

        await CreatePositionAsync(
            context,
            instrument.Id,
            10,
            225.75m);

        var order = await CreateOrderAsync(
            context,
            instrument.Id,
            "BUY",
            5,
            240m);

        var controller = new OrderExecutionsController(context);

        var result = await controller.ExecuteOrder(order.Id);

        Assert.IsType<OkObjectResult>(result);

        context.ChangeTracker.Clear();

        var savedOrder = await context.TradingOrders
            .AsNoTracking()
            .SingleAsync(o => o.Id == order.Id);

        var position = await context.PortfolioPositions
            .AsNoTracking()
            .SingleAsync(p =>
                p.FinancialInstrumentId == instrument.Id);

        Assert.Equal("EXECUTED", savedOrder.Status);
        Assert.Equal(15, position.Quantity);
        Assert.Equal(230.50m, position.AveragePrice);

        Assert.Equal(
            1,
            await context.PortfolioPositions.CountAsync());
    }

    // TEST 5: Partial SELL reduces quantity

    [Fact]
    public async Task ExecuteOrder_WhenSellingPartOfPosition_ReducesQuantity()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context =
            await CreateContextAsync(connection);

        var instrument = await CreateInstrumentAsync(context);

        await CreatePositionAsync(
            context,
            instrument.Id,
            10,
            225.75m);

        var order = await CreateOrderAsync(
            context,
            instrument.Id,
            "SELL",
            4,
            240m);

        var controller = new OrderExecutionsController(context);

        var result = await controller.ExecuteOrder(order.Id);

        Assert.IsType<OkObjectResult>(result);

        context.ChangeTracker.Clear();

        var savedOrder = await context.TradingOrders
            .AsNoTracking()
            .SingleAsync(o => o.Id == order.Id);

        var position = await context.PortfolioPositions
            .AsNoTracking()
            .SingleAsync(p =>
                p.FinancialInstrumentId == instrument.Id);

        Assert.Equal("EXECUTED", savedOrder.Status);
        Assert.Equal(6, position.Quantity);

        // Selling shares must not change acquisition price.
        Assert.Equal(225.75m, position.AveragePrice);
    }

    // TEST 6: Insufficient holdings -> HTTP 409

    [Fact]
    public async Task ExecuteOrder_WhenSellQuantityExceedsHoldings_ReturnsConflict()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context =
            await CreateContextAsync(connection);

        var instrument = await CreateInstrumentAsync(context);

        await CreatePositionAsync(
            context,
            instrument.Id,
            5,
            225.75m);

        var order = await CreateOrderAsync(
            context,
            instrument.Id,
            "SELL",
            10,
            240m);

        var controller = new OrderExecutionsController(context);

        var result = await controller.ExecuteOrder(order.Id);

        Assert.IsType<ConflictObjectResult>(result);

        context.ChangeTracker.Clear();

        var savedOrder = await context.TradingOrders
            .AsNoTracking()
            .SingleAsync(o => o.Id == order.Id);

        var position = await context.PortfolioPositions
            .AsNoTracking()
            .SingleAsync(p =>
                p.FinancialInstrumentId == instrument.Id);

        Assert.Equal("NEW", savedOrder.Status);
        Assert.Equal(5, position.Quantity);
        Assert.Equal(225.75m, position.AveragePrice);
    }

    // TEST 7: SELL without any holdings -> HTTP 409

    [Fact]
    public async Task ExecuteOrder_WhenSellingWithoutPosition_ReturnsConflict()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context =
            await CreateContextAsync(connection);

        var instrument = await CreateInstrumentAsync(context);

        var order = await CreateOrderAsync(
            context,
            instrument.Id,
            "SELL",
            3,
            225.75m);

        var controller = new OrderExecutionsController(context);

        var result = await controller.ExecuteOrder(order.Id);

        Assert.IsType<ConflictObjectResult>(result);

        context.ChangeTracker.Clear();

        var savedOrder = await context.TradingOrders
            .AsNoTracking()
            .SingleAsync(o => o.Id == order.Id);

        Assert.Equal("NEW", savedOrder.Status);

        Assert.Empty(
            await context.PortfolioPositions.ToListAsync());
    }

    // TEST 8: Full SELL closes the portfolio position

    [Fact]
    public async Task ExecuteOrder_WhenSellingAllShares_RemovesPosition()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context =
            await CreateContextAsync(connection);

        var instrument = await CreateInstrumentAsync(context);

        await CreatePositionAsync(
            context,
            instrument.Id,
            10,
            225.75m);

        var order = await CreateOrderAsync(
            context,
            instrument.Id,
            "SELL",
            10,
            240m);

        var controller = new OrderExecutionsController(context);

        var result = await controller.ExecuteOrder(order.Id);

        Assert.IsType<OkObjectResult>(result);

        context.ChangeTracker.Clear();

        var savedOrder = await context.TradingOrders
            .AsNoTracking()
            .SingleAsync(o => o.Id == order.Id);

        Assert.Equal("EXECUTED", savedOrder.Status);

        Assert.Empty(
            await context.PortfolioPositions.ToListAsync());

        // Order history must still exist after position closure.
        Assert.Equal(
            1,
            await context.TradingOrders.CountAsync());
    }

    // TESTS 9-11: Invalid order data -> HTTP 400
    // Each InlineData case is counted as a separate test.

    [Theory]
    [InlineData("BUY", 0, 100)]
    [InlineData("HOLD", 10, 100)]
    [InlineData("BUY", 10, 0)]
    public async Task ExecuteOrder_WhenOrderDataIsInvalid_ReturnsBadRequest(
        string orderType,
        int quantity,
        int price)
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context =
            await CreateContextAsync(connection);

        var instrument = await CreateInstrumentAsync(context);

        var order = await CreateOrderAsync(
            context,
            instrument.Id,
            orderType,
            quantity,
            price);

        var controller = new OrderExecutionsController(context);

        var result = await controller.ExecuteOrder(order.Id);

        Assert.IsType<BadRequestObjectResult>(result);

        context.ChangeTracker.Clear();

        var savedOrder = await context.TradingOrders
            .AsNoTracking()
            .SingleAsync(o => o.Id == order.Id);

        Assert.Equal("NEW", savedOrder.Status);

        Assert.Empty(
            await context.PortfolioPositions.ToListAsync());
    }

    // TEST 12: Already executed SELL cannot execute again

    [Fact]
    public async Task ExecuteOrder_WhenSellAlreadyExecuted_DoesNotReduceQuantityAgain()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context =
            await CreateContextAsync(connection);

        var instrument = await CreateInstrumentAsync(context);

        await CreatePositionAsync(
            context,
            instrument.Id,
            6,
            225.75m);

        var order = await CreateOrderAsync(
            context,
            instrument.Id,
            "SELL",
            4,
            240m,
            "EXECUTED");

        var controller = new OrderExecutionsController(context);

        var result = await controller.ExecuteOrder(order.Id);

        Assert.IsType<ConflictObjectResult>(result);

        context.ChangeTracker.Clear();

        var savedOrder = await context.TradingOrders
            .AsNoTracking()
            .SingleAsync(o => o.Id == order.Id);

        var position = await context.PortfolioPositions
            .AsNoTracking()
            .SingleAsync(p =>
                p.FinancialInstrumentId == instrument.Id);

        Assert.Equal("EXECUTED", savedOrder.Status);
        Assert.Equal(6, position.Quantity);
        Assert.Equal(225.75m, position.AveragePrice);
    }
}
