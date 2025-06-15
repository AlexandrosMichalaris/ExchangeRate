using ExchangeRate.Application.Constants;
using ExchangeRate.Application.Interface;
using ExchangeRate.Application.Service.Strategy;
using ExchangeRate.Application.Service.Wallet;
using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Model.Dto.External;
using ExchangeRate.Model.Entities;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;


public class WalletAppServiceTests
{
    private readonly Mock<IAdjustmentService> _adjustmentServiceMock;
    private readonly Mock<IExchangeRateRepository> _exchangeRateRepoMock;
    private readonly Mock<ILogger<WalletAppService>> _loggerMock;
    private readonly WalletAppService _walletAppService;

    public WalletAppServiceTests()
    {
        _adjustmentServiceMock = new Mock<IAdjustmentService>();
        _exchangeRateRepoMock = new Mock<IExchangeRateRepository>();
        _loggerMock = new Mock<ILogger<WalletAppService>>();

        _walletAppService = new WalletAppService(
            _adjustmentServiceMock.Object,
            _exchangeRateRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateWalletAsync_WithUnsupportedCurrency_ShouldFail()
    {
        // Arrange
        var dto = new CreateWalletRequestDto { Currency = "XYZ" };

        // Act
        var result = await _walletAppService.CreateWalletAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Unsupported currency");
    }

    [Fact]
    public async Task CreateWalletAsync_WithValidCurrency_ShouldSucceed()
    {
        // Arrange
        var dto = new CreateWalletRequestDto { Currency = "usd" };
        _adjustmentServiceMock.Setup(a => a.CreateWalletAsync("USD")).ReturnsAsync(123);

        // Act
        var result = await _walletAppService.CreateWalletAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWalletBalanceAsync_WalletNotFound_ShouldFail()
    {
        _adjustmentServiceMock.Setup(x => x.GetWalletAsync(It.IsAny<long>()))
            .ReturnsAsync((WalletEntity?)null);

        var result = await _walletAppService.GetWalletBalanceAsync(1, null);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Wallet not found");
    }

    [Fact]
    public async Task GetWalletBalanceAsync_WithCurrencyConversion_ShouldReturnConverted()
    {
        var wallet = new WalletEntity { Id = 1, Balance = 100m, Currency = "EUR" };

        _adjustmentServiceMock.Setup(a => a.GetWalletAsync(1)).ReturnsAsync(wallet);
        _exchangeRateRepoMock.Setup(r => r.GetExchangeRateAsync("EUR", "USD")).ReturnsAsync(1.5m);

        var result = await _walletAppService.GetWalletBalanceAsync(1, "USD");

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Currency.Should().Be("USD");
        result.Data.Balance.Should().Be(150.00m);
    }

    [Fact]
    public async Task AdjustWalletBalanceAsync_WithInvalidAmount_ShouldFail()
    {
        var dto = new AdjustBalanceRequestDto
        {
            Amount = 0,
            Currency = "EUR",
            Strategy = "Add"
        };

        var result = await _walletAppService.AdjustWalletBalanceAsync(1, dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Amount must be greater than zero");
    }

    [Fact]
    public async Task AdjustWalletBalanceAsync_WithInvalidCurrency_ShouldFail()
    {
        var dto = new AdjustBalanceRequestDto
        {
            Amount = 100,
            Currency = "ZZZ",
            Strategy = "Add"
        };

        var result = await _walletAppService.AdjustWalletBalanceAsync(1, dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Unsupported currency");
    }

    [Fact]
    public async Task AdjustWalletBalanceAsync_WithValidInput_ShouldSucceed()
    {
        var wallet = new WalletEntity { Id = 1, Currency = "EUR", Balance = 100m };

        var dto = new AdjustBalanceRequestDto
        {
            Amount = 50,
            Currency = "USD",
            Strategy = "AddFunds"
        };

        _adjustmentServiceMock.Setup(a => a.GetWalletAsync(1)).ReturnsAsync(wallet);
        _exchangeRateRepoMock.Setup(r => r.GetExchangeRateAsync("USD", "EUR")).ReturnsAsync(0.5m);
        _adjustmentServiceMock.Setup(a => a.AdjustBalanceAsync(1, 25, AdjustmentType.AddFunds))
            .Returns(Task.CompletedTask);

        var result = await _walletAppService.AdjustWalletBalanceAsync(1, dto);

        result.Success.Should().BeTrue();
        result.Message.Should().Be("OK");
    }

    [Fact]
    public async Task AdjustWalletBalanceAsync_WithInvalidStrategy_ShouldThrow()
    {
        var wallet = new WalletEntity { Id = 1, Currency = "EUR", Balance = 100m };
        var dto = new AdjustBalanceRequestDto
        {
            Amount = 100,
            Currency = "EUR",
            Strategy = "INVALID"
        };

        _adjustmentServiceMock.Setup(a => a.GetWalletAsync(1)).ReturnsAsync(wallet);

        var result = await _walletAppService.AdjustWalletBalanceAsync(1, dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid strategy type");
    }
}