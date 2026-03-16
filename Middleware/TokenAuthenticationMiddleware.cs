using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace UserManagementAPI.Middleware
{
    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ExpectedToken = "Bearer TechHiveSecureToken";

        public TokenAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip auth for Swagger UI for easier testing if needed, 
            // but requirements say "AUTHENTICATION MIDDLEWARE" for the API.
            // We'll apply it to all requests as requested.

            if (!context.Request.Headers.TryGetValue("Authorization", out var token) || token != ExpectedToken)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var response = new { error = "Unauthorized access" };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }

            await _next(context);
        }
    }
}
