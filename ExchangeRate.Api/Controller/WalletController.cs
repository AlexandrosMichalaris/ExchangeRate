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
    private readonly IWalletService _walletAppService;

    public WalletController(IWalletService walletAppService)
    {
        _walletAppService = walletAppService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequestDto dto)
    {
        var response = await _walletAppService.CreateWalletAsync(dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpGet("{walletId:long}")]
    public async Task<IActionResult> GetWallet(long walletId, [FromQuery] string? currency = null)
    {
        var response = await _walletAppService.GetWalletBalanceAsync(walletId, currency);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpPost("{walletId:long}/adjustbalance")]
    public async Task<IActionResult> AdjustBalance(long walletId, [FromBody] AdjustBalanceRequestDto dto)
    {
        var response = await _walletAppService.AdjustWalletBalanceAsync(walletId, dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }
}