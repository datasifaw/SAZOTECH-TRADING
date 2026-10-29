
namespace SAZOTECH.Trading.API.Models;

public class FinancialInstrument
{
    public int Id { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Exchange { get; set; } = string.Empty;

    public decimal LastPrice { get; set; }

    public string Currency { get; set; } = "EUR";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
