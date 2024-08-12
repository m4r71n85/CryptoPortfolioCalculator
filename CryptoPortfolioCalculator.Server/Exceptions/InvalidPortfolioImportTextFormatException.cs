namespace CryptoPortfolioCalculator.Server.Exceptions
{
    public class InvalidPortfolioImportTextFormatException: BasePortfolioException
    {
        public InvalidPortfolioImportTextFormatException(string message) : base(message)
        {
            
        }
    }
}
