namespace ExchangeRates.Application.Exceptions;

public class ExchangeRateFetchException : Exception
{
    public ExchangeRateFetchException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
