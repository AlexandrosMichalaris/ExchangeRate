namespace ExchangeRate.Model.Entities;

public class WalletTransactionEntity
{
    public long Id { get; set; }

    public long WalletId { get; set; }
    public WalletEntity WalletEntity { get; set; } = default!;

    public decimal Amount { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}