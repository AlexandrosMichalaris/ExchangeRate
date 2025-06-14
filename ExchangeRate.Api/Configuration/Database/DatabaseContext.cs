using ExchangeRate.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRate.Infrastructure.Configuration.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }
    
    public DbSet<ExchangeRateEntity> ExchangeRates { get; set;}
    public DbSet<WalletEntity> Wallets { get; set; }
    public DbSet<WalletTransactionEntity> WalletTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExchangeRateEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Currency, e.Date }).IsUnique();
            entity.Property(e => e.Currency).HasMaxLength(3).IsRequired();
        });

        modelBuilder.Entity<WalletEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Currency).HasMaxLength(3).IsRequired();
        });

        modelBuilder.Entity<WalletTransactionEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.WalletEntity)
                .WithMany()
                .HasForeignKey(e => e.WalletId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}