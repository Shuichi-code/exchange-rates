using ExchangeRates.Application.Exceptions;
using ExchangeRates.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRates.Api.Controllers;

[ApiController]
[Route("api/exchange-rates")]
public class ExchangeRatesController : ControllerBase
{
    private readonly IExchangeRateProvider _exchangeRateProvider;
    private readonly ILogger<ExchangeRatesController> _logger;

    public ExchangeRatesController(IExchangeRateProvider exchangeRateProvider, ILogger<ExchangeRatesController> logger)
    {
        _exchangeRateProvider = exchangeRateProvider;
        _logger = logger;
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _exchangeRateProvider.GetLatestAsync(cancellationToken);
            return Ok(response);
        }
        catch (ExchangeRateFetchException ex)
        {
            _logger.LogWarning(ex, "Failed to fetch CNB exchange rates.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                error = "CNB service unavailable",
                detail = ex.Message
            });
        }
        catch (ExchangeRateParseException ex)
        {
            _logger.LogWarning(ex, "Failed to parse CNB exchange rates.");
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                error = "Invalid CNB data",
                detail = ex.Message
            });
        }
    }
}
