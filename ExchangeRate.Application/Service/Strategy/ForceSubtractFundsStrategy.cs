using ExchangeRate.Application.Constants;
using ExchangeRate.Application.Interface;
using ExchangeRate.Model.Entities;
using Microsoft.Extensions.Logging;

namespace ExchangeRate.Application.Service.Strategy;

public class ForceSubtractFundsStrategy : IAdjustWalletStrategy
{
    private readonly ILogger<ForceSubtractFundsStrategy> _logger;
    
    public ForceSubtractFundsStrategy(ILogger<ForceSubtractFundsStrategy> logger)
    {
        _logger = logger;
    }
    
    public AdjustmentType Type => AdjustmentType.ForceSubtractFunds;
    
    
    public Task AdjustAsync(WalletEntity wallet, decimal amount)
    {
        try
        {
            wallet.Balance -= amount;
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in ForceSubtractFunds. Message: {message}", e.Message);
            throw;
        }
    }
}