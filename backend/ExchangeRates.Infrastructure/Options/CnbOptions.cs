namespace ExchangeRates.Infrastructure.Options;

public class CnbOptions
{
    public const string SectionName = "Cnb";

    public string DailyRatesUrl { get; set; } = string.Empty;
}
