using System;
using Microsoft.AspNetCore.Hosting;
using TwentyTwenty.Mvc;
using TwentyTwenty.Mvc.ErrorHandling;
using TwentyTwenty.Mvc.HealthCheck;

namespace Microsoft.AspNetCore.Builder
{
    public static class ErrorHandling
    {
        public static void UseErrorHandling(this IApplicationBuilder app, ICodeMap codeMap)
            => app.UseErrorHandling(codeMap.MapErrorCode);

        public static void UseErrorHandling(this IApplicationBuilder app, Func<int, int> codeMap = null)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>(codeMap);
        }

        public static void UseHealthCheck(this IApplicationBuilder app)
            => app.UseHealthCheck("/status/health-check");

        public static void UseHealthCheck(this IApplicationBuilder app, string path)
        {
            app.Map(path, builder =>
            {
                builder.UseMiddleware<HealthCheckMiddleware>();
            });
        }
    }
}