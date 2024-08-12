using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using CryptoPortfolioCalculator.Server.Services;
using System.Text;
using static CryptoPortfolioCalculator.Server.Controllers.PortfolioController;
using Moq.Protected;
using System.Net;
using CryptoPortfolioCalculator.Server.Exceptions;

namespace CryptoPortfolioCalculator.UnitTests
{
    [TestFixture]
    public class PortfolioFileServiceTests
    {
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private Mock<ILogger<PortfolioFileService>> _mockLogger;
        private PortfolioFileService _service;

        [SetUp]
        public void Setup()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<PortfolioFileService>>();
            _service = new PortfolioFileService(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        [Test]
        public void ParseContent_ValidContent_ReturnsCorrectPortfolioItems()
        {
            // Arrange
            var content = "10|ETH|123.14\n12.12454|BTC|24012.43";

            // Act
            var result = _service.ParseContent(content);

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Amount, Is.EqualTo(10m));
            Assert.That(result[0].Coin, Is.EqualTo("ETH"));
            Assert.That(result[0].InitialPrice, Is.EqualTo(123.14m));
            Assert.That(result[1].Amount, Is.EqualTo(12.12454m));
            Assert.That(result[1].Coin, Is.EqualTo("BTC"));
            Assert.That(result[1].InitialPrice, Is.EqualTo(24012.43m));
        }


        [Test]
        public async Task CalculatePortfolioValueAsync_ValidData_ReturnsCorrectSummary()
        {
            // Arrange
            List<PortfolioItem> portfolioItems =
            [
                new() { Amount = 10, Coin = "ETH", InitialPrice = 100m },
                new() { Amount = 1, Coin = "BTC", InitialPrice = 20000m }
            ];

            var mockResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"data\":[{\"symbol\":\"ETH\",\"price_usd\":150},{\"symbol\":\"BTC\",\"price_usd\":25000}]}")
            };

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse);

            var client = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act
            var result = await _service.CalculatePortfolioValueAsync(portfolioItems);

            // Assert
            Assert.AreEqual(21000m, result.InitialPortfolioValue);
            Assert.AreEqual(26500m, result.CurrentPortfolioValue);
            Assert.AreEqual(26.19m, Math.Round(result.OverallChange, 2));
        }

        [Test]
        public void ParseContent_InvalidFormat_ThrowsException()
        {
            // Arrange
            var content = "10,ETH,123.14\n12.12454,BTC,24012.43"; // Using commas instead of pipes

            // Act & Assert
            Assert.Throws<InvalidPortfolioImportTextFormatException>(() => _service.ParseContent(content));
        }

        [Test]
        public async Task CalculatePortfolioValueAsync_ApiReturnsError_ThrowsException()
        {
            // Arrange
            var portfolioItems = new List<PortfolioItem>
    {
        new PortfolioItem { Amount = 10, Coin = "ETH", InitialPrice = 100m }
    };

            var mockResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal Server Error")
            };

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse);

            var client = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act & Assert
            Assert.ThrowsAsync<ErrorRetrievingPortfolioDataException>(() => _service.CalculatePortfolioValueAsync(portfolioItems));
        }
    }
}