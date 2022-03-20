using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BikeTheft.Application.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var response = context.HttpContext.Response;

            //We can switch on exception types here like Domain Ex. or Application Ex.
            switch (context.Exception)
            {
                default:
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Result = new BadRequestObjectResult("An error occured.");
                    break;
            }

            _logger.Log(LogLevel.Error,
                        new EventId(context.Exception.HResult,
                        context.Exception.GetType().Name),
                        context.Exception,
                        "Api exception is handled.");

            context.ExceptionHandled = true;
        }
    }
}
