using ExchangeRate.Application.Constants;
using ExchangeRate.Application.Interface;

namespace ExchangeRate.Application.Service.Strategy;

public class WalletStrategyFactory :IWalletStrategyFactory
{
    private readonly IEnumerable<IAdjustWalletStrategy> _handlers;

    public WalletStrategyFactory(IEnumerable<IAdjustWalletStrategy> handlers)
    {
        _handlers = handlers;
    }
    
    public IAdjustWalletStrategy GetStrategy(AdjustmentType strategy)
    {
        var implementation = _handlers.SingleOrDefault(x => x.Type == strategy);
        
        if(implementation is null)
            throw new ApplicationException($"{nameof(WalletStrategyFactory)} - Invalid strategy type");
        
        return implementation;
    }
}