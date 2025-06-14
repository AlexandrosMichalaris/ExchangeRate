using ExchangeRate.Application.Constants;

namespace ExchangeRate.Application.Helper;

public static class CurrencyHelper
{
    public static bool IsSupported(string currencyCode)
    {
        return Enum.TryParse(typeof(SupportedCurrency), currencyCode.ToUpper(), out _);
    }

    public static string Normalize(string currencyCode)
    {
        return currencyCode.ToUpper();
    }
}