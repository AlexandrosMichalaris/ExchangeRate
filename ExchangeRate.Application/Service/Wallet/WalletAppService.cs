using ExchangeRate.Application.Constants;
using ExchangeRate.Application.Helper;
using ExchangeRate.Application.Interface;
using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Model.Dto.External;
using Microsoft.Extensions.Logging;
using Model.ApiResponse;

namespace ExchangeRate.Application.Service.Wallet;

public class WalletAppService : IWalletAppService
{
    private readonly ILogger<WalletAppService> _logger;
    private readonly IAdjustmentService _adjustmentService;
    private readonly IExchangeRateRepository _exchangeRateRepository;

    public WalletAppService(IAdjustmentService adjustmentService, IExchangeRateRepository exchangeRateRepository, ILogger<WalletAppService> logger)
    {
        _adjustmentService = adjustmentService;
        _exchangeRateRepository = exchangeRateRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<object>> CreateWalletAsync(CreateWalletRequestDto dto)
    {
        if (!CurrencyHelper.IsSupported(dto.Currency))
            return new ApiResponse<object>(null, false, "Unsupported currency");

        var walletId = await _adjustmentService.CreateWalletAsync(CurrencyHelper.Normalize(dto.Currency));
        return new ApiResponse<object>(new { walletId }, "Wallet created successfully");
    }

    public async Task<ApiResponse<WalletBalanceResponseDto>> GetWalletBalanceAsync(long walletId, string? currency)
    {
        var wallet = await _adjustmentService.GetWalletAsync(walletId);
        if (wallet == null)
            return new ApiResponse<WalletBalanceResponseDto>(null, false, "Wallet not found");

        var targetCurrency = string.IsNullOrWhiteSpace(currency)
            ? wallet.Currency
            : CurrencyHelper.Normalize(currency);

        if (!CurrencyHelper.IsSupported(targetCurrency))
            return new ApiResponse<WalletBalanceResponseDto>(null, false, "Unsupported currency");

        var convertedBalance = wallet.Balance;

        if (targetCurrency != wallet.Currency)
        {
            var rate = await _exchangeRateRepository.GetExchangeRateAsync(wallet.Currency, targetCurrency);
            if (rate == null)
                return new ApiResponse<WalletBalanceResponseDto>(null, false, "Exchange rate not available");

            convertedBalance *= rate.Value;
        }

        return new ApiResponse<WalletBalanceResponseDto>(new WalletBalanceResponseDto
        {
            WalletId = wallet.Id,
            Balance = Math.Round(convertedBalance, 2),
            Currency = targetCurrency
        }, "Wallet balance retrieved");
    }

    public async Task<ApiResponse<string>> AdjustWalletBalanceAsync(long walletId, AdjustBalanceRequestDto dto)
    {
        if (dto.Amount <= 0)
            return new ApiResponse<string>(null, false, "Amount must be greater than zero");

        if (!CurrencyHelper.IsSupported(dto.Currency))
            return new ApiResponse<string>(null, false, "Unsupported currency");

        try
        {
            var wallet = await _adjustmentService.GetWalletAsync(walletId);
            if (wallet == null)
                return new ApiResponse<string>(null, false, "Wallet not found");

            var fromCurrency = CurrencyHelper.Normalize(dto.Currency);
            var toCurrency = wallet.Currency;

            var adjustedAmount = dto.Amount;

            if (fromCurrency != toCurrency)
            {
                var rate = await _exchangeRateRepository.GetExchangeRateAsync(fromCurrency, toCurrency);
                if (rate == null)
                    return new ApiResponse<string>(null, false, "Exchange rate not available");

                adjustedAmount *= rate.Value;
            }
            
            if (!Enum.TryParse<AdjustmentType>(dto.Strategy, ignoreCase: true, out var strategy))
            {
                throw new InvalidOperationException($"Invalid strategy type: {dto.Strategy}");
            }

            await _adjustmentService.AdjustBalanceAsync(walletId, adjustedAmount, strategy);

            return new ApiResponse<string>("Balance adjusted successfully", "OK");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error in AdjustWalletBalanceAsync. Message: {message}", ex.Message);
            return new ApiResponse<string>(null, false, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AdjustWalletBalanceAsync. Message: {message}", ex.Message);
            return new ApiResponse<string>(null, false, "Internal error: " + ex.Message);
        }
    }
}