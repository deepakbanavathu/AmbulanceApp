using System.Net;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace Ambulance.Api.MiddleWear
{
    public class ExceptionMiddleware
    {
       private readonly RequestDelegate _next;
       private readonly ILogger<ExceptionMiddleware> _logger;

       public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

       public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(UnauthorizedAccessException ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                //Add Custom header for mobile app
                string message = ex.Message;
                switch(message)
                {
                    case "token_expired":
                        context.Response.Headers["X-Token-Status"] = "expired";
                        break;
                    case "Invalid_token":
                        context.Response.Headers["X-Token-Status"] = "invalid";
                        break;
                    default:
                        context.Response.Headers["X-Token-Status"] = "unauthorized";
                        break;
                }

                await context.Response.WriteAsJsonAsync(new {error = ex.Message});
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, $"something went wrong: {ex.Message}");
                await HandleExceptionAsync(context,ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error.",
                Detailed = ex.Message
            };

            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
    }
}
