using ExchangeRate.Application.Constants;

namespace ExchangeRate.Application.Interface;

public interface IWalletStrategyFactory
{
    IAdjustWalletStrategy GetStrategy(AdjustmentType strategy);
}