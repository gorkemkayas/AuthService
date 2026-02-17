using AuthService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

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

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                using var scope = serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var env = scope.ServiceProvider
                    .GetService<IHostEnvironment>()?.EnvironmentName ?? "Unknown";

                var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

                var (controller, action) = GetControllerAction(context);
                var fileDisplay = GetSourceLocation(ex);

                var time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'");

                var subject = "🚨 AuthService | Unhandled Exception";

                var body = BuildHtmlExceptionBody(
                    ex,
                    context,
                    controller,
                    action,
                    fileDisplay,
                    env,
                    traceId,
                    time
                );

                await emailService.SendEmailAsync("gorkemkayas@hotmail.com", subject, body);

                _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

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
        private static (string Controller, string Action) GetControllerAction(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
                return ("Unknown", "Unknown");

            var cad = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (cad == null)
                return ("Unknown", "Unknown");

            return (cad.ControllerName, cad.ActionName);
        }
        private static string GetSourceLocation(Exception ex)
        {
            var stack = ex.StackTrace ?? string.Empty;

            var firstRelevant = stack
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(l => l.Contains(" in ") && l.Contains(":line"));

            if (firstRelevant == null)
                return "Unknown";

            var fileMatch = Regex.Match(
                firstRelevant,
                @"in\s+(?<path>.*?):line\s*(?<line>\d+)",
                RegexOptions.Singleline);

            if (!fileMatch.Success)
                return "Unknown";

            var path = fileMatch.Groups["path"].Value;
            var line = fileMatch.Groups["line"].Value;

            var fileName = Path.GetFileName(path);
            return $"{fileName}:{line}";
        }
        private static string BuildHtmlExceptionBody(Exception ex,HttpContext context,string controller,string action,string fileDisplay,string env,string traceId,string time)
        {
            return $@"
<h2 style='color:#b91c1c'>🚨 AUTH SERVICE — UNHANDLED EXCEPTION</h2>
<hr/>

<h3>📌 Summary</h3>
<p><b>Message:</b><br/>{WebUtility.HtmlEncode(ex.Message)}</p>

<h3>📍 Location</h3>
<ul>
  <li><b>Controller:</b> {controller}</li>
  <li><b>Action:</b> {action}</li>
  <li><b>Source:</b> {fileDisplay}</li>
</ul>

<h3>🌐 Request</h3>
<ul>
  <li><b>Method:</b> {context.Request.Method}</li>
  <li><b>Path:</b> {context.Request.Path}</li>
</ul>

<h3>🖥 Environment</h3>
<ul>
  <li><b>Environment:</b> {env}</li>
  <li><b>Time (UTC):</b> {time}</li>
  <li><b>TraceId:</b> {traceId}</li>
</ul>";
        }


    }
}
