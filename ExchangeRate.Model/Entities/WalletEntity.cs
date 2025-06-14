using System.ComponentModel.DataAnnotations;

namespace ExchangeRate.Model.Entities;

public class WalletEntity
{
    public long Id { get; set; }

    public decimal Balance { get; set; }

    [MaxLength(3)]
    public string Currency { get; set; } = "EUR";
}