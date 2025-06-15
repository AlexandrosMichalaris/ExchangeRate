using AutoMapper;
using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Model.Entities;
using Microsoft.Extensions.Logging;
using Quartz;

namespace ExchangeRate.Application.Service;

public class UpdateExchangeRatesJob : IJob
{
    private readonly IExchangeRateProvider _rateProvider;
    private readonly IExchangeRateRepository _rateRepository;
    private readonly ILogger<UpdateExchangeRatesJob> _logger;

    public UpdateExchangeRatesJob(
        IExchangeRateProvider rateProvider,
        IExchangeRateRepository rateRepository)
    {
        _rateProvider = rateProvider;
        _rateRepository = rateRepository;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var rateDtos = await _rateProvider.GetLatestRatesAsync();
            var domainRates = rateDtos.Select(r => new ExchangeRateEntity()
            {
                Currency = r.Currency,
                Rate = r.Rate,
                Date = r.Date
            }).ToList();

            await _rateRepository.UpsertExchangeRatesAsync(domainRates);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in the periodic retrieval of rates inside service. Message: {message}", e.Message);
            throw;
        }
    }
}