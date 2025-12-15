using ExchangeRates.Application.Exceptions;
using ExchangeRates.Application.Interfaces;
using ExchangeRates.Application.Models;
using ExchangeRates.Infrastructure.Options;
using ExchangeRates.Infrastructure.Parsing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExchangeRates.Infrastructure.Providers;

public class CnbExchangeRateProvider : IExchangeRateProvider
{
    private const string CacheKey = "cnb-latest-rates";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    private readonly HttpClient _httpClient;
    private readonly CnbDailyRatesParser _parser;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CnbExchangeRateProvider> _logger;
    private readonly string _dailyRatesUrl;

    public CnbExchangeRateProvider(
        HttpClient httpClient,
        CnbDailyRatesParser parser,
        IMemoryCache cache,
        IOptions<CnbOptions> options,
        ILogger<CnbExchangeRateProvider> logger)
    {
        _httpClient = httpClient;
        _parser = parser;
        _cache = cache;
        _logger = logger;
        _dailyRatesUrl = options.Value.DailyRatesUrl ?? string.Empty;

        if (string.IsNullOrWhiteSpace(_dailyRatesUrl))
        {
            throw new ArgumentException("Cnb:DailyRatesUrl is not configured.", nameof(options));
        }
    }

    public async Task<ExchangeRatesResponse> GetLatestAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey, out ExchangeRatesResponse? cached) && cached is not null)
        {
            return cached;
        }

        var rawData = await FetchRawDataAsync(cancellationToken);
        var parsed = Parse(rawData);

        _cache.Set(CacheKey, parsed, CacheDuration);
        return parsed;
    }

    private async Task<string> FetchRawDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _httpClient.GetStringAsync(_dailyRatesUrl, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch CNB exchange rates from {Url}", _dailyRatesUrl);
            throw new ExchangeRateFetchException("Failed to fetch CNB exchange rates.", ex);
        }
    }

    private ExchangeRatesResponse Parse(string rawData)
    {
        try
        {
            return _parser.Parse(rawData);
        }
        catch (ExchangeRateParseException ex)
        {
            _logger.LogError(ex, "Failed to parse CNB exchange rates.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when parsing CNB exchange rates.");
            throw new ExchangeRateParseException("Failed to parse CNB exchange rates.", ex);
        }
    }
}
