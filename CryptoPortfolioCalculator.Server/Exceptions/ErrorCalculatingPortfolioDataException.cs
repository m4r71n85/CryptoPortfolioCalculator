namespace CryptoPortfolioCalculator.Server.Exceptions
{
    public class ErrorCalculatingPortfolioDataException : BasePortfolioException
    {
        public ErrorCalculatingPortfolioDataException(string message) : base(message) { }
    }
}
