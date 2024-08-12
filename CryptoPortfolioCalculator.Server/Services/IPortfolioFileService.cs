using static CryptoPortfolioCalculator.Server.Controllers.PortfolioController;

namespace CryptoPortfolioCalculator.Server.Services
{
    public interface IPortfolioFileService
    {
        List<PortfolioItem> ParseContent(string contents);
        Task<PortfolioSummary> CalculatePortfolioValueAsync(List<PortfolioItem> portfolio);
    }
}