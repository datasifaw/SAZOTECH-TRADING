
namespace SAZOTECH.Trading.API.Models;

public class TradingOrder
{
    public int Id { get; set; }

    public int FinancialInstrumentId { get; set; }

    public FinancialInstrument FinancialInstrument { get; set; } = null!;

    public string OrderType { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "NEW";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
