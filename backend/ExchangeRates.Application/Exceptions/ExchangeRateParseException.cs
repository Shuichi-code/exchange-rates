namespace ExchangeRates.Application.Exceptions;

public class ExchangeRateParseException : Exception
{
    public ExchangeRateParseException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
