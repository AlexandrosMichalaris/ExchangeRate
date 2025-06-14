using ExchangeRate.Application.Constants;
using ExchangeRate.Application.Service.Strategy;
using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Model.Entities;

namespace ExchangeRate.Application.Interface;

public interface IAdjustmentService
{
    Task AdjustBalanceAsync(long walletId, decimal amount, AdjustmentType strategy);

    Task<WalletEntity> GetWalletAsync(long walletId);

    Task<long> CreateWalletAsync(string currency);
}