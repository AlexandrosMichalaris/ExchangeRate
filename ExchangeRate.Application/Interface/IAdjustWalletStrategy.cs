using ExchangeRate.Application.Constants;
using ExchangeRate.Model.Entities;

namespace ExchangeRate.Application.Interface;

public interface IAdjustWalletStrategy
{
    public AdjustmentType Type { get; }
    
    Task AdjustAsync(WalletEntity wallet, decimal amount);
}