using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace AuthService.API.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}",context.Request.Method, context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                // ARGUMENT ERRORS (framework veya pipeline kaynaklı)
                ArgumentNullException ex =>
                    new ErrorResponse(HttpStatusCode.BadRequest, $"Argument null: {ex.ParamName}"),

                ArgumentException ex =>
                    new ErrorResponse(HttpStatusCode.BadRequest, ex.Message),

                // NOT IMPLEMENTED
                NotImplementedException =>
                    new ErrorResponse(HttpStatusCode.NotImplemented, "Feature not implemented."),

                // TIMEOUT / Operation hataları
                TimeoutException =>
                    new ErrorResponse(HttpStatusCode.RequestTimeout, "The operation timed out."),

                // DATABASE / EF CORE hataları
                DbUpdateException =>
                    new ErrorResponse(HttpStatusCode.Conflict, "Database update failed."),

                // 3RD PARTY API hataları
                HttpRequestException =>
                    new ErrorResponse(HttpStatusCode.BadGateway, "External service call failed."),

                // JSON / Model binding hataları
                System.Text.Json.JsonException =>
                    new ErrorResponse(HttpStatusCode.BadRequest, "Invalid JSON in request body."),

                // NULL REFERENCE / UNEXPECTED BUGS
                NullReferenceException =>
                    new ErrorResponse(HttpStatusCode.InternalServerError, "Unexpected null reference occurred."),

                InvalidOperationException =>
                    new ErrorResponse(HttpStatusCode.InternalServerError, "Invalid operation detected."),

                // FALLBACK (diğer bilinmeyen teknik hatalar)
                _ =>
                    new ErrorResponse(
                        HttpStatusCode.InternalServerError,
                        "An unexpected technical error occurred."
                    )
            };



            context.Response.StatusCode = (int)response.StatusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        private record ErrorResponse(HttpStatusCode StatusCode, string Message, List<string>? Errors = null);
    }
}
