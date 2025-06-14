using ExchangeRate.Application.Constants;
using ExchangeRate.Application.Interface;
using ExchangeRate.Application.Service.Strategy;
using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Model.Entities;

namespace ExchangeRate.Application.Service.Wallet;

public class AdjustmentService : IAdjustmentService
{
    private readonly IWalletRepository _walletRepo;
    private readonly IWalletTransaction _walletTransaction;
    private readonly WalletStrategyFactory _strategyFactory;

    public AdjustmentService(IWalletRepository walletRepo, WalletStrategyFactory strategyFactory, IWalletTransaction walletTransaction)
    {
        _walletRepo = walletRepo;
        _strategyFactory = strategyFactory;
        _walletTransaction = walletTransaction;
    }

    public async Task AdjustBalanceAsync(long walletId, decimal amount, AdjustmentType strategy)
    {
        var wallet = await _walletRepo.GetByIdAsync(walletId);
        if (wallet == null)
            throw new Exception("Wallet not found");

        var strategyHandler = _strategyFactory.GetStrategy(strategy);
        await strategyHandler.AdjustAsync(wallet, amount);

        await _walletRepo.UpdateAsync(wallet);
        
        var transaction = new WalletTransactionEntity
        {
            WalletId = walletId,
            Amount = amount
        };

        await _walletTransaction.AddAsync(transaction);
    }

    public async Task<WalletEntity> GetWalletAsync(long walletId)
    {
        var wallet = await _walletRepo.GetByIdAsync(walletId);
        if (wallet == null)
            throw new Exception("Wallet not found");
        return wallet;
    }

    public async Task<long> CreateWalletAsync(string currency)
    {
        var wallet = new WalletEntity() { Balance = 0m, Currency = currency };
        await _walletRepo.AddAsync(wallet);
        return wallet.Id;
    }
}