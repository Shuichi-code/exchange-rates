using ExchangeRates.Application.Models;

namespace ExchangeRates.Application.Interfaces;

public interface IExchangeRateProvider
{
    Task<ExchangeRatesResponse> GetLatestAsync(CancellationToken cancellationToken = default);
}
