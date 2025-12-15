using ExchangeRates.Infrastructure.Parsing;
using Xunit;

namespace ExchangeRates.Tests;

public class CnbDailyRatesParserTests
{
    private const string SamplePayload = """
15.12.2025 #244
zeme|mena|mnozstvi|kod|kurz
Australia|dollar|1|AUD|15,742
Brazil|real|1|BRL|4,897
""";

    private readonly CnbDailyRatesParser _parser = new();

    [Fact]
    public void Parses_AsOfDate_From_First_Line()
    {
        var result = _parser.Parse(SamplePayload);

        Assert.Equal(new DateTime(2025, 12, 15), result.AsOfDate);
    }

    [Fact]
    public void Parses_At_Least_Two_Rows()
    {
        var result = _parser.Parse(SamplePayload);

        Assert.Equal(2, result.Rates.Count);
        Assert.Collection(
            result.Rates,
            first =>
            {
                Assert.Equal("Australia", first.Country);
                Assert.Equal("dollar", first.CurrencyName);
                Assert.Equal(1, first.Amount);
                Assert.Equal("AUD", first.Code);
                Assert.Equal(15.742m, first.RateCzk);
            },
            second =>
            {
                Assert.Equal("Brazil", second.Country);
                Assert.Equal("real", second.CurrencyName);
                Assert.Equal(1, second.Amount);
                Assert.Equal("BRL", second.Code);
                Assert.Equal(4.897m, second.RateCzk);
            });
    }

    [Fact]
    public void Parses_Comma_Decimal_To_Decimal_Correctly()
    {
        var result = _parser.Parse(SamplePayload);

        Assert.All(result.Rates, rate => Assert.True(rate.RateCzk > 0));
        Assert.Equal(15.742m, result.Rates.First(r => r.Code == "AUD").RateCzk);
    }
}
