namespace CryptoPortfolioCalculator.Server.Exceptions
{
    public class ErrorRetrievingPortfolioDataException : BasePortfolioException
    {
        public ErrorRetrievingPortfolioDataException(string message) : base(message) { }
    }
}
