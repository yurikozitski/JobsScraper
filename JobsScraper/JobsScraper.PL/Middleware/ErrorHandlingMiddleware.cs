using Newtonsoft.Json;
using System.Net;

namespace JobsScraper.PL.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> _logger)
        {
            _next = next;
            logger = _logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, logger);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger<ErrorHandlingMiddleware> _logger)
        {
            string? message = "";

            switch (ex)
            {
                case Exception:
                    _logger.LogError(ex, "Server error");
                    message = "An unhandled exception has occurred.";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";

            if (message != "")
            {
                var result = JsonConvert.SerializeObject(new
                {
                    ErrorMessage = message,
                });

                await context.Response.WriteAsync(result);
            }
        }
    }
}
