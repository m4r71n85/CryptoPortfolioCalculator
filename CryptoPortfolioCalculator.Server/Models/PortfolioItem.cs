namespace CryptoPortfolioCalculator.Server.Controllers
{
    public partial class PortfolioController
    {
        public class PortfolioItem
        {
            public decimal Amount { get; set; }
            public string Coin { get; set; }
            public decimal InitialPrice { get; set; }
            public decimal? CurrentPrice { get; set; }
            public decimal? Change { get; set; }
        }
    }
}