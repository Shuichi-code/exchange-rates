namespace ExchangeRates.Application.Models;

public class ExchangeRateRow
{
    public string Country { get; set; } = string.Empty;

    public string CurrencyName { get; set; } = string.Empty;

    public int Amount { get; set; }

    public string Code { get; set; } = string.Empty;

    public decimal RateCzk { get; set; }
}
