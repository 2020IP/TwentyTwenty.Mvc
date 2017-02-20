using Microsoft.AspNetCore.Hosting;
using TwentyTwenty.Mvc.ErrorHandling;

namespace Microsoft.AspNetCore.Builder
{
    public static class ErrorHandling
    {
        public static void UserErrorHandling(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}