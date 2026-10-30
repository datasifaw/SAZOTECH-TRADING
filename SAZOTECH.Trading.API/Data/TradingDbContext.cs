
using Microsoft.EntityFrameworkCore;
using SAZOTECH.Trading.API.Models;

namespace SAZOTECH.Trading.API.Data;

public class TradingDbContext : DbContext
{
    public TradingDbContext(
        DbContextOptions<TradingDbContext> options)
        : base(options)
    {
    }

    public DbSet<FinancialInstrument> FinancialInstruments
        => Set<FinancialInstrument>();

    public DbSet<TradingOrder> TradingOrders
        => Set<TradingOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Financial Instruments configuration
        modelBuilder.Entity<FinancialInstrument>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(e => e.Symbol)
                .IsUnique();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Exchange)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Currency)
                .IsRequired()
                .HasMaxLength(3);

            entity.Property(e => e.LastPrice)
                .HasPrecision(18, 4);
        });

        // Trading Orders configuration
        modelBuilder.Entity<TradingOrder>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OrderType)
                .IsRequired()
                .HasMaxLength(4);

            entity.Property(e => e.Quantity)
                .IsRequired();

            entity.Property(e => e.Price)
                .HasPrecision(18, 4);

            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 4);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("NEW");

            entity.HasOne(e => e.FinancialInstrument)
                .WithMany()
                .HasForeignKey(e => e.FinancialInstrumentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.FinancialInstrumentId);
        });
    }
}
