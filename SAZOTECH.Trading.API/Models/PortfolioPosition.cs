
namespace SAZOTECH.Trading.API.Models;

public class PortfolioPosition
{
    public int Id { get; set; }

    // Financial instrument linked to this position
    public int FinancialInstrumentId { get; set; }

    public FinancialInstrument FinancialInstrument { get; set; } = null!;

    // Number of shares currently held
    public int Quantity { get; set; }

    // Weighted average acquisition price
    public decimal AveragePrice { get; set; }

    // Date of the latest position update
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
