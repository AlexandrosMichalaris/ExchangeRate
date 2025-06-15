using ExchangeRate.Application.Interface;
using ExchangeRate.Controller;
using ExchangeRate.Model.Dto.External;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Model.ApiResponse;
using Moq;
using Xunit;

namespace ExchangeRate.Test;

public class WalletControllerTests
{
    private readonly Mock<IWalletAppService> _walletAppServiceMock;
    private readonly WalletController _controller;

    public WalletControllerTests()
    {
        _walletAppServiceMock = new Mock<IWalletAppService>();
        _controller = new WalletController(_walletAppServiceMock.Object);
    }

    [Fact]
    public async Task CreateWallet_Returns200_WhenSuccess()
    {
        // Arrange
        var dto = new CreateWalletRequestDto { Currency = "USD" };
        var apiResponse = new ApiResponse<object>(new { walletId = 1L }, "Wallet created successfully");

        _walletAppServiceMock
            .Setup(s => s.CreateWalletAsync(dto))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _controller.CreateWallet(dto);

        // Assert
        var okResult = result as ObjectResult;
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(apiResponse);
    }

    [Fact]
    public async Task CreateWallet_Returns400_WhenFailed()
    {
        var dto = new CreateWalletRequestDto { Currency = "XYZ" };
        var apiResponse = new ApiResponse<object>(null, false, "Unsupported currency");

        _walletAppServiceMock
            .Setup(s => s.CreateWalletAsync(dto))
            .ReturnsAsync(apiResponse);

        var result = await _controller.CreateWallet(dto);

        var badRequest = result as ObjectResult;
        badRequest!.StatusCode.Should().Be(400);
        badRequest.Value.Should().BeEquivalentTo(apiResponse);
    }

    [Fact]
    public async Task GetWallet_Returns200_WhenSuccess()
    {
        long walletId = 1;
        var currency = "USD";
        var response = new ApiResponse<WalletBalanceResponseDto>(
            new WalletBalanceResponseDto { WalletId = walletId, Balance = 100.0m, Currency = "USD" },
            "OK");

        _walletAppServiceMock
            .Setup(s => s.GetWalletBalanceAsync(walletId, currency))
            .ReturnsAsync(response);

        var result = await _controller.GetWallet(walletId, currency);

        var ok = result as ObjectResult;
        ok!.StatusCode.Should().Be(200);
        ok.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task AdjustBalance_Returns400_WhenValidationFails()
    {
        var walletId = 1L;
        var dto = new AdjustBalanceRequestDto
        {
            Amount = -100,
            Currency = "USD",
            Strategy = "Increase"
        };

        var response = new ApiResponse<string>(null, false, "Amount must be greater than zero");

        _walletAppServiceMock
            .Setup(s => s.AdjustWalletBalanceAsync(walletId, dto))
            .ReturnsAsync(response);

        var result = await _controller.AdjustBalance(walletId, dto);

        var bad = result as ObjectResult;
        bad!.StatusCode.Should().Be(400);
        bad.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task AdjustBalance_Returns200_WhenSuccess()
    {
        var walletId = 1L;
        var dto = new AdjustBalanceRequestDto
        {
            Amount = 100,
            Currency = "USD",
            Strategy = "Increase"
        };

        var response = new ApiResponse<string>("Balance adjusted successfully", "OK");

        _walletAppServiceMock
            .Setup(s => s.AdjustWalletBalanceAsync(walletId, dto))
            .ReturnsAsync(response);

        var result = await _controller.AdjustBalance(walletId, dto);

        var ok = result as ObjectResult;
        ok!.StatusCode.Should().Be(200);
        ok.Value.Should().BeEquivalentTo(response);
    }
}