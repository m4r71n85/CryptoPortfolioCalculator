namespace CryptoPortfolioCalculator.Server.Exceptions
{
    abstract public class BasePortfolioException : Exception
    {
        public int StatusCode { get; }

        protected BasePortfolioException(string message, int statusCode = StatusCodes.Status500InternalServerError) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
