using ExchangeRate.Application.Interface;
using ExchangeRate.Application.Service;
using ExchangeRate.Application.Service.Strategy;
using ExchangeRate.Application.Service.Wallet;
using ExchangeRate.Infrastructure.Configuration.Database;
using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Infrastructure.Service.Http;
using ExchangeRate.Infrastructure.Service.Repositories;
using ExchangeRate.Infrastructure.Service.Repositories.WalletRepository;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ExchangeRate.Configuration.DI;

public static class DiConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddHttpClient<IExchangeRateProvider, EcbExchangeRateProvider>();
        
        services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
        services.AddHttpClient<IExchangeRateProvider, EcbExchangeRateProvider>();

        // services.AddDbContext<DatabaseContext>(options =>
        // {
        //     var connectionString = configuration.GetConnectionString("DefaultConnection");
        //     options.UseNpgsql(connectionString, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "exchange"));
        // });

        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
        services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
        services.AddScoped<IAdjustmentService, AdjustmentService>();
        
        
        services.AddScoped<IWalletAppService, WalletAppService>();
        services.AddScoped<IExchangeRateProvider, EcbExchangeRateProvider>();

        services.AddScoped<IAdjustWalletStrategy, AddFundsStrategy>();
        services.AddScoped<IAdjustWalletStrategy, SubtractFundsStrategy>();
        services.AddScoped<IAdjustWalletStrategy, ForceSubtractFundsStrategy>();
        services.AddScoped<IWalletStrategyFactory, WalletStrategyFactory>();
        
        services.AddScoped<UpdateExchangeRatesJob>();
    }
}