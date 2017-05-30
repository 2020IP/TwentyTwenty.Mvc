using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TwentyTwenty.Mvc.ReadOnlyMode
{
    public class ReadOnlyModeMiddleware
    {
        private readonly RequestDelegate _next;
        
        public ReadOnlyModeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method != HttpMethod.Get.Method && context.Request.Method != HttpMethod.Options.Method)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Application is currently in read only mode.");
                return;
            }

            await _next(context);
        }
    }
}