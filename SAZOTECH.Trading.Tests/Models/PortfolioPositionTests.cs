
using SAZOTECH.Trading.API.Models;
using Xunit;

namespace SAZOTECH.Trading.Tests.Models;

public class PortfolioPositionTests
{
    [Fact]
    public void PortfolioPosition_ShouldStoreQuantityAndAveragePrice()
    {
        // Arrange
        var position = new PortfolioPosition
        {
            Id = 1,
            FinancialInstrumentId = 10,
            Quantity = 15,
            AveragePrice = 230.50m
        };

        // Act
        var quantity = position.Quantity;
        var averagePrice = position.AveragePrice;

        // Assert
        Assert.Equal(1, position.Id);
        Assert.Equal(10, position.FinancialInstrumentId);
        Assert.Equal(15, quantity);
        Assert.Equal(230.50m, averagePrice);
    }

    [Fact]
    public void PortfolioPosition_ShouldInitializeUpdatedAt()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var position = new PortfolioPosition();

        var after = DateTime.UtcNow;

        // Assert
        Assert.InRange(position.UpdatedAt, before, after);
    }

    [Fact]
    public void PortfolioPosition_ShouldAllowQuantityUpdate()
    {
        // Arrange
        var position = new PortfolioPosition
        {
            FinancialInstrumentId = 1,
            Quantity = 10,
            AveragePrice = 225.75m
        };

        // Act
        position.Quantity = 6;

        // Assert
        Assert.Equal(6, position.Quantity);
        Assert.Equal(225.75m, position.AveragePrice);
    }
}
