using ExchangeRate.Model.Dto.External;

namespace ExchangeRate.Infrastructure.Interface;

public interface IExchangeRateProvider
{
    Task<List<ExchangeRateDto>> GetLatestRatesAsync();
}