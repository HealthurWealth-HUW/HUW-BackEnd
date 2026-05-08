using HealthUrWealth.Api.Common;
using HealthUrWelath.Application.Common.Exceptions;
using Serilog;
using System.Text.Json;
using FluentValidation;

namespace HealthUrWealth.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
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

        private async Task HandleException(HttpContext context, Exception ex)
        {
            var controller = context.GetRouteValue("controller");
            var action = context.GetRouteValue("action");
            var route = context.Request.Path;
            var method = context.Request.Method;

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
                        "Validation Failed",
                        validationEx.Errors.Select(e => new
                        {
                            e.PropertyName,
                            e.ErrorMessage
                        }));
                    break;

                case AppException appEx:
                    statusCode = appEx.StatusCode;
                    response = ApiResponse<object>.Fail(appEx.Message);
                    break;

                case UnauthorizedAccessException:
                    statusCode = 401;
                    response = ApiResponse<object>.Fail("Unauthorized access");
                    break;

                default:
                    statusCode = 500;
                    response = ApiResponse<object>.Fail(
                        _env.IsDevelopment()
                            ? ex.Message
                            : "An unexpected error occurred");
                    break;
            }

            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
