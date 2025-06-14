using ExchangeRate.Application.Constants;

namespace ExchangeRate.Application.Interface;

public interface IWalletStrategyFactory
{
    IAdjustWallet GetStrategy(AdjustmentType strategy);
}