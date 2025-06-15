using ExchangeRate.Application.Interface;
using ExchangeRate.Application.Service.Wallet;
using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Model.Dto.External;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRate.Controller;

[ApiController]
[Route("api/wallets")]
public class WalletController : ControllerBase
{
    private readonly IWalletAppService _walletAppAppService;

    public WalletController(IWalletAppService walletAppAppService)
    {
        _walletAppAppService = walletAppAppService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequestDto dto)
    {
        var response = await _walletAppAppService.CreateWalletAsync(dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpGet("{walletId:long}")]
    public async Task<IActionResult> GetWallet(long walletId, [FromQuery] string? currency = null)
    {
        var response = await _walletAppAppService.GetWalletBalanceAsync(walletId, currency);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpPost("{walletId:long}/adjustbalance")]
    public async Task<IActionResult> AdjustBalance(long walletId, [FromQuery] AdjustBalanceRequestDto dto)
    {
        var response = await _walletAppAppService.AdjustWalletBalanceAsync(walletId, dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }
}