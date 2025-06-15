using System.Net;
using ExchangeRate.Infrastructure.Configuration;
using ExchangeRate.Infrastructure.Service.Http;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace ExchangeRate.Test;

public class EcbExchangeRateProviderTests
{
    private const string EcbXmlSample = @"
<gesmes:Envelope xmlns:gesmes='http://www.gesmes.org/xml/2002-08-01' 
                 xmlns='http://www.ecb.int/vocabulary/2002-08-01/eurofxref'>
    <Cube>
        <Cube time='2025-06-13'>
            <Cube currency='USD' rate='1.1512'/>
            <Cube currency='JPY' rate='165.94'/>
        </Cube>
    </Cube>
</gesmes:Envelope>";

    private readonly Mock<ILogger<EcbExchangeRateProvider>> _loggerMock;
    private readonly IOptions<EcbEuropaSettings> _settings;

    public EcbExchangeRateProviderTests()
    {
        _loggerMock = new Mock<ILogger<EcbExchangeRateProvider>>();
        _settings = Options.Create(new EcbEuropaSettings
        {
            BaseUrl = "https://fake-ecb.int/latest.xml"
        });
    }

    [Fact]
    public async Task GetLatestRatesAsync_ShouldReturnParsedRates_WhenXmlIsValid()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(_settings.Value.BaseUrl)
                .Respond("application/xml", EcbXmlSample);

        var httpClient = mockHttp.ToHttpClient();
        var provider = new EcbExchangeRateProvider(httpClient, _loggerMock.Object, _settings);

        // Act
        var result = await provider.GetLatestRatesAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Currency.Should().Be("USD");
        result[0].Rate.Should().Be(1.1512m);
        result[0].Date.Should().Be(new DateTime(2025, 6, 13));
    }

    [Fact]
    public async Task GetLatestRatesAsync_ShouldThrow_WhenHttpRequestFails()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(_settings.Value.BaseUrl)
                .Respond(HttpStatusCode.InternalServerError);

        var httpClient = mockHttp.ToHttpClient();
        var provider = new EcbExchangeRateProvider(httpClient, _loggerMock.Object, _settings);

        // Act
        Func<Task> action = async () => await provider.GetLatestRatesAsync();

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetLatestRatesAsync_ShouldThrow_WhenXmlIsMalformed()
    {
        // Arrange
        var malformedXml = "<notxml>";
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(_settings.Value.BaseUrl)
                .Respond("application/xml", malformedXml);

        var httpClient = mockHttp.ToHttpClient();
        var provider = new EcbExchangeRateProvider(httpClient, _loggerMock.Object, _settings);

        // Act
        Func<Task> action = async () => await provider.GetLatestRatesAsync();

        // Assert
        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GetLatestRatesAsync_ShouldThrow_WhenCubeElementMissing()
    {
        // Arrange
        var xmlWithoutCube = @"
<gesmes:Envelope xmlns:gesmes='http://www.gesmes.org/xml/2002-08-01' 
                 xmlns='http://www.ecb.int/vocabulary/2002-08-01/eurofxref'>
    <Cube></Cube>
</gesmes:Envelope>";

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(_settings.Value.BaseUrl)
                .Respond("application/xml", xmlWithoutCube);

        var httpClient = mockHttp.ToHttpClient();
        var provider = new EcbExchangeRateProvider(httpClient, _loggerMock.Object, _settings);

        // Act
        Func<Task> action = async () => await provider.GetLatestRatesAsync();

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("ECB XML does not contain expected data");
    }
}