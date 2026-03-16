using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace UserManagementAPI.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception details internally (simulated with Console log)
                Console.WriteLine($"[EXCEPTION] {DateTime.UtcNow}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new { error = "Internal server error." };
            var json = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(json);
        }
    }
}
