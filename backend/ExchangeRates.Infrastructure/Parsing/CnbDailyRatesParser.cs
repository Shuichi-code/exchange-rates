using System.Globalization;
using ExchangeRates.Application.Exceptions;
using ExchangeRates.Application.Models;

namespace ExchangeRates.Infrastructure.Parsing;

public class CnbDailyRatesParser
{
    private const string BaseCurrency = "CZK";
    private static readonly CultureInfo CzechCulture = CultureInfo.GetCultureInfo("cs-CZ");

    public ExchangeRatesResponse Parse(string rawData)
    {
        if (string.IsNullOrWhiteSpace(rawData))
        {
            throw new ExchangeRateParseException("CNB response was empty.");
        }

        var lines = rawData
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(line => line.TrimEnd('\r'))
            .ToArray();

        if (lines.Length < 3)
        {
            throw new ExchangeRateParseException("CNB response did not contain expected lines.");
        }

        var asOfDate = ParseDate(lines[0]);

        var rows = new List<ExchangeRateRow>();
        for (var i = 2; i < lines.Length; i++)
        {
            rows.Add(ParseRow(lines[i]));
        }

        return new ExchangeRatesResponse
        {
            AsOfDate = asOfDate,
            BaseCurrency = BaseCurrency,
            Rates = rows
        };
    }

    private static readonly CultureInfo EnglishCulture = CultureInfo.GetCultureInfo("en-US");

    private static DateTime ParseDate(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            throw new ExchangeRateParseException("CNB response did not include a date line.");

        // Example CZ: "15.12.2025 #241"
        // Example EN: "12 Dec 2025 #241"
        // Strip trailing "#..." if present
        var cleaned = line.Split('#', 2, StringSplitOptions.TrimEntries)[0].Trim();

        // Try Czech format first
        if (DateTime.TryParseExact(cleaned, "dd.MM.yyyy", CzechCulture, DateTimeStyles.None, out var czDate))
            return czDate.Date;

        // Try English formats
        var enFormats = new[] { "d MMM yyyy", "dd MMM yyyy" };
        if (DateTime.TryParseExact(cleaned, enFormats, EnglishCulture, DateTimeStyles.AllowWhiteSpaces, out var enDate))
            return enDate.Date;

        // As a last resort, try a normal parse with invariant culture
        if (DateTime.TryParse(cleaned, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var fallback))
            return fallback.Date;

        throw new ExchangeRateParseException($"Unable to parse CNB date line '{line}'.");
    }


    private static ExchangeRateRow ParseRow(string line)
    {
        var parts = line.Split('|', StringSplitOptions.TrimEntries);
        if (parts.Length < 5)
        {
            throw new ExchangeRateParseException("CNB data row did not contain 5 parts.");
        }

        if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var amount))
        {
            throw new ExchangeRateParseException($"Unable to parse amount '{parts[2]}'.");
        }

        if (!decimal.TryParse(parts[4], NumberStyles.Number, CzechCulture, out var rate))
        {
            var normalizedRate = parts[4].Replace(',', '.');
            if (!decimal.TryParse(normalizedRate, NumberStyles.Number, CultureInfo.InvariantCulture, out rate))
            {
                throw new ExchangeRateParseException($"Unable to parse rate '{parts[4]}'.");
            }
        }

        return new ExchangeRateRow
        {
            Country = parts[0],
            CurrencyName = parts[1],
            Amount = amount,
            Code = parts[3],
            RateCzk = rate
        };
    }
}
