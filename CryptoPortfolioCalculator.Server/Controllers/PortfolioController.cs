using CryptoPortfolioCalculator.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace CryptoPortfolioCalculator.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class PortfolioController : ControllerBase
    {
        private readonly ILogger<PortfolioController> _logger;
        private readonly IPortfolioFileService _portfolioService;

        public PortfolioController(IPortfolioFileService portfolioService, ILogger<PortfolioController> logger)
        {
            _logger = logger;
            _portfolioService = portfolioService;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculatePortfolio(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            using var reader = new StreamReader(file.OpenReadStream());
            var content = await reader.ReadToEndAsync();
            var portfolio = _portfolioService.ParseContent(content);
            var summary = await _portfolioService.CalculatePortfolioValueAsync(portfolio);

            return Ok(summary);
        }

        
    }
}