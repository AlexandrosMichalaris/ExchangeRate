using ExchangeRate.Model.Entities;

namespace ExchangeRate.Infrastructure.Interface;

public interface IExchangeRateRepository : IEntityRepository<ExchangeRateEntity>
{
    Task UpsertExchangeRatesAsync(List<ExchangeRateEntity> rates);

    Task<decimal?> GetExchangeRateAsync(string fromCurrency, string toCurrency);
}