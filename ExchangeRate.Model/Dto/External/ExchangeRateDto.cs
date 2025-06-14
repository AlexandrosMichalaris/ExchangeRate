namespace ExchangeRate.Model.Dto.External;

public class ExchangeRateDto
{
    public string Currency { get; set; }
    
    public decimal Rate { get; set; }
    
    public DateTime Date { get; set; }
}