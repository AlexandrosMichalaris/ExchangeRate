using System.Globalization;
using System.Xml.Linq;
using ExchangeRate.Infrastructure.Configuration;
using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Model.Dto.External;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExchangeRate.Infrastructure.Service.Http;

public class EcbExchangeRateProvider : IExchangeRateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EcbExchangeRateProvider> _logger;
    private readonly EcbEuropaSettings _settings;
    
    
    public EcbExchangeRateProvider(
        HttpClient httpClient,
        ILogger<EcbExchangeRateProvider> logger,
        IOptions<EcbEuropaSettings> settings)
    {
        _httpClient = httpClient;
        _logger = logger;
        _settings = settings.Value;
    }
    
    public async Task<List<ExchangeRateDto>> GetLatestRatesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(_settings.BaseUrl);
            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var document = XDocument.Parse(xmlContent);

            var ns = document.Root!.GetDefaultNamespace();

            var cube = document.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "Cube" && e.Attribute("time") != null);

            if (cube == null)
                throw new InvalidOperationException("ECB XML does not contain expected data");

            var date = DateTime.Parse(cube.Attribute("time")!.Value);

            var rates = cube.Elements()
                .Select(e => new ExchangeRateDto
                {
                    Currency = e.Attribute("currency")!.Value,
                    Rate = decimal.Parse(e.Attribute("rate")!.Value, CultureInfo.InvariantCulture),
                    Date = date
                })
                .ToList();

            return rates;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in the retrieval of the rates endpoint. Message: {message}", e.Message);
            throw;
        }
    }
}