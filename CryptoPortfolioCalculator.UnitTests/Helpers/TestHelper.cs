using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace CryptoPortfolioCalculator.UnitTests.Helpers
{
    public class TestHelper
    {
        public static IFormFile CreateMockFile(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(bytes.Length);
            file.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(bytes));
            return file.Object;
        }
    }
}
