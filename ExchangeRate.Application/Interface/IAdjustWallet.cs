using ExchangeRate.Application.Constants;
using ExchangeRate.Model.Entities;

namespace ExchangeRate.Application.Interface;

public interface IAdjustWallet
{
    public AdjustmentType Type { get; }
    
    Task AdjustAsync(WalletEntity wallet, decimal amount);
}