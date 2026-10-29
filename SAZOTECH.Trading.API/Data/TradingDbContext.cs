
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
    }
}
