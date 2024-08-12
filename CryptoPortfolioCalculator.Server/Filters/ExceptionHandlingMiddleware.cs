using CryptoPortfolioCalculator.Server.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace CryptoPortfolioCalculator.Server.Filters
{
    public class ExceptionHandlingMiddleware : IExceptionFilter
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An unhandled exception has occurred");

            var response = context.HttpContext.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                Message = context.Exception is BasePortfolioException
                    ? context.Exception.Message
                    : "An unexpected error occurred. Please try again later.",
                Type = context.Exception.GetType().Name
            };

            if (context.Exception is BasePortfolioException portfolioException)
            {
                response.StatusCode = portfolioException.StatusCode;
            }
            else
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            context.Result = new ContentResult
            {
                Content = JsonSerializer.Serialize(errorResponse),
                ContentType = "application/json",
                StatusCode = response.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public string Type { get; set; }
    }

    //public override void OnException(ExceptionContext exceptionContext)
    //{

    //    var errorMessage = exceptionContext.Exception is BasePortfolioException ? exceptionContext.Exception.Message : "Server error. Please contact administrators.";
    //    exceptionContext.Result = new ObjectResult(errorMessage)
    //    {
    //        StatusCode = StatusCodes.Status500InternalServerError
    //    };

    //    base.OnException(exceptionContext);
    //}
}
