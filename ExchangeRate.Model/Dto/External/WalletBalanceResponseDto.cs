namespace ExchangeRate.Model.Dto.External;

public class WalletBalanceResponseDto
{
    public long WalletId { get; set; }
    
    public decimal Balance { get; set; }
    
    public string Currency { get; set; }
}