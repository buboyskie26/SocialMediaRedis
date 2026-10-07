using Social_Media.Exceptions;
using System.Net;
using System.Text.Json;

namespace Social_Media.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                UnauthorizedAccessExceptionClass ex => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message,
                    Details = "Authentication is required to access this resource."
                },
                ForbiddenException ex => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    Message = ex.Message,
                    Details = "You don't have permission to access this resource."
                },
                NotFoundException ex => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = ex.Message,
                    Details = "The requested resource was not found."
                },
                BadRequestException ex => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Details = "The request contains invalid data."
                },
                _ => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "An unexpected error occurred.",
                    Details = exception.Message // Only in Development
                }
            };

            context.Response.StatusCode = response.StatusCode;

            // Log the error
            if (response.StatusCode >= 500)
            {
                _logger.LogError(exception, "Server Error: {Message}", exception.Message);
            }
            else
            {
                _logger.LogWarning(exception, "Client Error: {Message}", exception.Message);
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    // Error Response Model
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
