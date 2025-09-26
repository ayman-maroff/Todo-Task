using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace TodoApp.Infrastructure.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
                if (context.Response.StatusCode == 401)
                {
                    await WriteJsonResponse(context, 401, "Token is missing or invalid.");
                }
                else if (context.Response.StatusCode == 403)
                {
                    await WriteJsonResponse(context, 403, "You do not have permission to perform this action.");
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred");

            var statusCode = exception switch
            {
                InvalidOperationException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            var message = exception.Message ?? "An unexpected error occurred.";

            return WriteJsonResponse(context, (int)statusCode, message);
        }

        private static Task WriteJsonResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var responseObject = new
            {
                statusCode,
                message
            };

            var jsonResponse = JsonSerializer.Serialize(responseObject);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
