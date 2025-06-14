using ExchangeRate.Model.Dto.External;
using Model.ApiResponse;

namespace ExchangeRate.Application.Interface;

public interface IWalletService
{
    Task<ApiResponse<object>> CreateWalletAsync(CreateWalletRequestDto dto);
    
    Task<ApiResponse<WalletBalanceResponseDto>> GetWalletBalanceAsync(long walletId, string? currency);
    
    Task<ApiResponse<string>> AdjustWalletBalanceAsync(long walletId, AdjustBalanceRequestDto dto);
}