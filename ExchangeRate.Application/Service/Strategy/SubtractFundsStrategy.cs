using ExchangeRate.Application.Constants;
using ExchangeRate.Application.Interface;
using ExchangeRate.Model.Entities;
using Microsoft.Extensions.Logging;

namespace ExchangeRate.Application.Service.Strategy;

public class SubtractFundsStrategy : IAdjustWalletStrategy
{
    private readonly ILogger<SubtractFundsStrategy> _logger;
    
    public SubtractFundsStrategy(ILogger<SubtractFundsStrategy> logger)
    {
        _logger = logger;
    }
    
    public AdjustmentType Type => AdjustmentType.SubtractFunds;
    
    
    public Task AdjustAsync(WalletEntity wallet, decimal amount)
    {
        try
        {
            if (wallet.Balance < amount)
                throw new InvalidOperationException("Insufficient funds");

            wallet.Balance -= amount;
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in Subtraction. Message: {message}", e.Message);
            throw;
        }
    }
}