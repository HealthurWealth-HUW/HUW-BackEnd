using FluentValidation;
using HealthUrWealth.Api.Common;
using HealthUrWelath.Application.Common.Exceptions;
using HealthUrWelath.Application.Notifications.Models;
using HealthUrWelath.Infrastructure.Notifications.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Text.Json;

namespace HealthUrWealth.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            IWebHostEnvironment env,
            IConfiguration config)
        {
            _next = next;
            _env = env;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private async Task HandleException(HttpContext context, Exception ex)
        {
            var controller = context.GetRouteValue("controller");
            var action = context.GetRouteValue("action");
            var route = context.Request.Path;
            var method = context.Request.Method;
            var traceId = context.TraceIdentifier;

            Log.Error(ex,
                "Unhandled Exception | Controller: {Controller} | Action: {Action} | Method: {Method} | Route: {Route}",
                controller,
                action,
                method,
                route);

            context.Response.ContentType = "application/json";

            ApiResponse<object> response;
            int statusCode;

            switch (ex)
            {
                case ValidationException validationEx:
                    statusCode = 400;
                    response = ApiResponse<object>.Fail(
                        "Validation failed.",
                        statusCode,
                        validationEx.Errors
                            .Select(e => new ApiError(e.ErrorMessage, e.PropertyName))
                            .ToList(),
                        traceId);
                    break;

                case AppException appEx:
                    statusCode = appEx.StatusCode;
                    response = ApiResponse<object>.Fail(appEx.Message, statusCode, traceId: traceId);
                    break;

                case UnauthorizedAccessException unauthorizedEx:
                    statusCode = 401;
                    response = ApiResponse<object>.Fail(unauthorizedEx.Message, statusCode, traceId: traceId);
                    break;

                default:
                    statusCode = 500;
                    response = ApiResponse<object>.Fail(
                        _env.IsDevelopment()
                            ? ex.Message
                            : "An unexpected error occurred",
                        statusCode,
                        new List<ApiError> { new(_env.IsDevelopment() ? ex.Message : "Internal Server Error.") },
                        traceId);

                    await TrySendExceptionAlertAsync(context, ex, method, route.ToString(), controller, action);
                    break;
            }

            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, SerializerOptions));
        }

        private async Task TrySendExceptionAlertAsync(
            HttpContext context,
            Exception ex,
            object? method,
            string? route,
            object? controller,
            object? action)
        {
            var alertEnabled = _config.GetValue<bool>("ExceptionAlert:Enabled");
            var toEmail = _config["ExceptionAlert:ToEmail"];

            if (!alertEnabled || string.IsNullOrWhiteSpace(toEmail))
                return;

            try
            {
                var emailService = context.RequestServices.GetService<IEmailService>();
                if (emailService is null)
                    return;

                await emailService.SendAsync(new EmailMessage
                {
                    To = toEmail,
                    Subject = $"[HUW] Unhandled Exception: {ex.GetType().Name}",
                    Body = BuildAlertBody(ex, method, route, controller, action),
                    IsHtml = true
                });
            }
            catch (Exception alertEx)
            {
                Log.Warning(alertEx, "Exception alert email failed to send");
            }
        }

        private static string BuildAlertBody(
            Exception ex,
            object? method,
            string? route,
            object? controller,
            object? action)
        {
            var message = HtmlEncode(ex.Message);
            var stackTrace = HtmlEncode(ex.StackTrace);
            var innerHtml = ex.InnerException is not null
                ? $"<h3>Inner Exception</h3><pre style=\"background:#f4f4f4;padding:12px;font-size:12px\">{HtmlEncode(ex.InnerException.ToString())}</pre>"
                : string.Empty;

            return $"""
                <h2 style="color:#c0392b">Unhandled Exception</h2>
                <table style="border-collapse:collapse;font-family:Arial,sans-serif;font-size:14px">
                  <tr><td style="padding:4px 12px 4px 0"><b>Time (UTC)</b></td><td>{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}</td></tr>
                  <tr><td style="padding:4px 12px 4px 0"><b>Method</b></td><td>{method} {route}</td></tr>
                  <tr><td style="padding:4px 12px 4px 0"><b>Controller</b></td><td>{controller} / {action}</td></tr>
                  <tr><td style="padding:4px 12px 4px 0"><b>Exception</b></td><td>{ex.GetType().FullName}</td></tr>
                  <tr><td style="padding:4px 12px 4px 0"><b>Message</b></td><td>{message}</td></tr>
                </table>
                <h3>Stack Trace</h3>
                <pre style="background:#f4f4f4;padding:12px;font-size:12px;overflow:auto">{stackTrace}</pre>
                {innerHtml}
                """;
        }

        private static string HtmlEncode(string? value) =>
            value is null ? string.Empty : value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;");
    }
}
