using ExchangeRate.Application.Service;
using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Infrastructure.Service.Http;
using ExchangeRate.Infrastructure.Service.Repositories;
using Quartz;

namespace ExchangeRate.Configuration.DI;

public static class DiConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddHttpClient<IExchangeRateProvider, EcbExchangeRateProvider>();
        
        services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
        services.AddHttpClient<IExchangeRateProvider, EcbExchangeRateProvider>();
        
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("UpdateExchangeRatesJob");

            q.AddJob<UpdateExchangeRatesJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("UpdateExchangeRatesJob-trigger")
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever()));
        });
    }
}