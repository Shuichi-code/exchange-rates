namespace ExchangeRates.Application.Models;

public class ExchangeRatesResponse
{
    public DateTime AsOfDate { get; set; }

    public string BaseCurrency { get; set; } = string.Empty;

    public IReadOnlyList<ExchangeRateRow> Rates { get; set; } = Array.Empty<ExchangeRateRow>();
}
