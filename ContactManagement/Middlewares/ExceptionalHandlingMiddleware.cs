using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Threading.Tasks;

namespace ContactManagement.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionalHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionalHandlingMiddleware> _logger;

        private readonly IDiagnosticContext _diagnosticContext;

        public ExceptionalHandlingMiddleware(RequestDelegate next, ILogger<ExceptionalHandlingMiddleware> logger, IDiagnosticContext diagnosticContext)
        {
            _next = next; // denotes the next middleware in the pipeline
            _logger = logger; // logger instance
            _diagnosticContext = diagnosticContext; // diagnostic context instance
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Exception Type : {ex.InnerException.GetType().ToString()} and Exception : {ex.InnerException.Message}");
                }
                else
                {
                    _logger.LogError($"Exception Type : {ex.GetType().ToString()} and Exception : {ex.Message}");
                }
                // comment this so that we can go to built in exception route
                //httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError; // set the response status code to 500
                //await httpContext.Response.WriteAsync("An error occurred while processing your request. Please try again later."); // This is a generic error message to the user
                throw;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionalHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionalHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionalHandlingMiddleware>();
        }
    }
}
