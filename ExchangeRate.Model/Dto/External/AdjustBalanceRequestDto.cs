namespace ExchangeRate.Model.Dto.External;

public class AdjustBalanceRequestDto
{
    public decimal Amount { get; set; }
    
    public string Currency { get; set; }
    
    public string Strategy { get; set; }
}