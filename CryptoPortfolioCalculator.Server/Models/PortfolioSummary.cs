namespace CryptoPortfolioCalculator.Server.Controllers
{
    public partial class PortfolioController
    {
        public class PortfolioSummary
        {
            public List<PortfolioItem> Items { get; set; } = [];
            public decimal InitialPortfolioValue { get; set; }
            public decimal CurrentPortfolioValue { get; set; }
            public decimal OverallChange { get; set; }
        }
    }
}