using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TwentyTwenty.Mvc.HealthCheck
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        
        public HealthCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await context.Response.WriteAsync("OK");
        }
    }
}