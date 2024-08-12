using static CryptoPortfolioCalculator.Server.Controllers.PortfolioController;
using System.Globalization;
using Newtonsoft.Json;
using CryptoPortfolioCalculator.Server.Exceptions;

namespace CryptoPortfolioCalculator.Server.Services
{
    public class PortfolioFileService : IPortfolioFileService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<PortfolioFileService> _logger;

        public PortfolioFileService(IHttpClientFactory clientFactory, ILogger<PortfolioFileService> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public List<PortfolioItem> ParseContent(string contents)
        {
            try
            {
                return contents.Split('\n')
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line =>
                    {
                        var parts = line.Split('|');
                        return new PortfolioItem
                        {
                            Amount = decimal.Parse(parts[0].Trim(), CultureInfo.InvariantCulture),
                            Coin = parts[1],
                            InitialPrice = decimal.Parse(parts[2].Trim(), CultureInfo.InvariantCulture)
                        };
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while parcing portfolio text data");
                throw new InvalidPortfolioImportTextFormatException("Error while parcing portfolio text data");
            }
        }

        public async Task<PortfolioSummary> CalculatePortfolioValueAsync(List<PortfolioItem> portfolioItems)
        {
            CoinDataResponse? retrievedCoinData;
            try
            {
                retrievedCoinData = await RetrieveCoindData(portfolioItems);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error while retrieving coin data", ex);
                throw new ErrorRetrievingPortfolioDataException("Error while retrieving coin data");
            }

            try
            {
                if (retrievedCoinData?.Data == null)
                {
                    return new();
                }

                foreach (var item in portfolioItems)
                {
                    var coinData = retrievedCoinData.Data.FirstOrDefault(d => d.Symbol == item.Coin);
                    if (coinData != null)
                    {
                        item.CurrentPrice = decimal.Parse(coinData.PriceUsd.ToString(), CultureInfo.InvariantCulture);
                        item.Change = ((item.CurrentPrice - item.InitialPrice) / item.InitialPrice) * 100;
                    }
                }

                var initialValue = portfolioItems.Sum(item => item.Amount * item.InitialPrice);
                var currentValue = portfolioItems.Sum(item => item.Amount * (item.CurrentPrice ?? item.InitialPrice));

                return new PortfolioSummary
                {
                    Items = portfolioItems,
                    InitialPortfolioValue = initialValue,
                    CurrentPortfolioValue = currentValue,
                    OverallChange = ((currentValue - initialValue) / initialValue) * 100
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while calculating portfolio data", ex);
                throw new InvalidPortfolioImportTextFormatException("Error while calculating portfolio data");
            }
        }

        private async Task<CoinDataResponse?> RetrieveCoindData(List<PortfolioItem> portfolioItems)
        {
            var client = _clientFactory.CreateClient();
            var symbols = string.Join(",", portfolioItems.Select(p => p.Coin));
            var response = await client.GetAsync($"https://api.coinlore.net/api/tickers/?symbol={symbols}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            CoinDataResponse? coinDataResponse = JsonConvert.DeserializeObject<CoinDataResponse>(content);

            return coinDataResponse;
        }
    }
}