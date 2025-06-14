using System.ComponentModel.DataAnnotations;

namespace ExchangeRate.Model.Entities;

public class ExchangeRateEntity
{
    public int Id { get; set; }

    [MaxLength(3)]
    public string Currency { get; set; } = default!;

    public decimal Rate { get; set; }

    public DateTime Date { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}