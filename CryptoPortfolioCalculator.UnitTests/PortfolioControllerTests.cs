using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CryptoPortfolioCalculator.Server.Controllers;
using CryptoPortfolioCalculator.Server.Services;
using static CryptoPortfolioCalculator.Server.Controllers.PortfolioController;
using CryptoPortfolioCalculator.UnitTests.Helpers;
using CryptoPortfolioCalculator.Server.Exceptions;

namespace CryptoPortfolioCalculator.UnitTests
{
    [TestFixture]
    public class PortfolioControllerTests
    {
        private Mock<IPortfolioFileService> _mockService;
        private Mock<ILogger<PortfolioController>> _mockLogger;
        private PortfolioController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IPortfolioFileService>();
            _mockLogger = new Mock<ILogger<PortfolioController>>();
            _controller = new PortfolioController(_mockService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task CalculatePortfolio_ValidFile_ReturnsOkResult()
        {
            // Arrange
            var content = "10|ETH|123.14\n12.12454|BTC|24012.43";
            var file = TestHelper.CreateMockFile(content);

            var portfolioItems = new List<PortfolioItem>
            {
                new PortfolioItem { Amount = 10, Coin = "ETH", InitialPrice = 123.14m },
                new PortfolioItem { Amount = 12.12454m, Coin = "BTC", InitialPrice = 24012.43m }
            };

            var summary = new PortfolioSummary
            {
                Items = portfolioItems,
                InitialPortfolioValue = 292000m,
                CurrentPortfolioValue = 300000m,
                OverallChange = 2.74m
            };

            _mockService.Setup(s => s.ParseContent(It.IsAny<string>())).Returns(portfolioItems);
            _mockService.Setup(s => s.CalculatePortfolioValueAsync(It.IsAny<List<PortfolioItem>>()))
                .ReturnsAsync(summary);

            // Act
            var result = await _controller.CalculatePortfolio(file);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.IsInstanceOf<PortfolioSummary>(okResult.Value);
            var returnedSummary = (PortfolioSummary)okResult.Value;
            Assert.AreEqual(summary.InitialPortfolioValue, returnedSummary.InitialPortfolioValue);
            Assert.AreEqual(summary.CurrentPortfolioValue, returnedSummary.CurrentPortfolioValue);
            Assert.AreEqual(summary.OverallChange, returnedSummary.OverallChange);
        }

        [Test]
        public async Task CalculatePortfolio_EmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var file = TestHelper.CreateMockFile(string.Empty);

            // Act
            var result = await _controller.CalculatePortfolio(file);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task CalculatePortfolio_InvalidFileFormat_ReturnsBadRequest()
        {
            // Arrange
            var content = "10,ETH,123.14\n12.12454,BTC,24012.43"; // Commas instead of pipes
            var file = TestHelper.CreateMockFile(content);

            _mockService.Setup(s => s.ParseContent(It.IsAny<string>()))
                .Throws(new InvalidPortfolioImportTextFormatException("Invalid file format"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidPortfolioImportTextFormatException>(async () => await _controller.CalculatePortfolio(file));
        }

        [Test]
        public async Task CalculatePortfolio_ApiError_ReturnsInternalServerError()
        {
            // Arrange
            var content = "10|ETH|123.14\n12.12454|BTC|24012.43";
            var file = TestHelper.CreateMockFile(content);

            _mockService.Setup(s => s.ParseContent(It.IsAny<string>())).Returns(new List<PortfolioItem>());
            _mockService.Setup(s => s.CalculatePortfolioValueAsync(It.IsAny<List<PortfolioItem>>()))
                .ThrowsAsync(new ErrorRetrievingPortfolioDataException("API error"));

            // Act & Assert
            Assert.ThrowsAsync<ErrorRetrievingPortfolioDataException>(async () => await _controller.CalculatePortfolio(file));
        }


    }
}