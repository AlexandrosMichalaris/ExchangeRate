using ExchangeRate.Application.Constants;
using ExchangeRate.Application.Interface;
using ExchangeRate.Model.Entities;
using Microsoft.Extensions.Logging;

namespace ExchangeRate.Application.Service.Strategy;

public class AddFunds : IAdjustWallet
{
    private readonly ILogger<AddFunds> _logger;
    
    public AddFunds(ILogger<AddFunds> logger)
    {
        _logger = logger;
    }
    
    public AdjustmentType Type => AdjustmentType.AddFunds;
    
    public Task AdjustAsync(WalletEntity wallet, decimal amount)
    {
        try
        {
            wallet.Balance += amount;
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in Add Funds. Message: {message}", e.Message);
            throw;
        }
    }
}