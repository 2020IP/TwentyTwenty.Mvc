using System;
using Microsoft.Extensions.Options;
using TwentyTwenty.Mvc;
using TwentyTwenty.Mvc.Correlation;
using TwentyTwenty.Mvc.ErrorHandling;
using TwentyTwenty.Mvc.HealthCheck;
using TwentyTwenty.Mvc.ReadOnlyMode;
using TwentyTwenty.Mvc.TokenBlacklist;
using TwentyTwenty.Mvc.Version;

namespace Microsoft.AspNetCore.Builder
{
    public static class ErrorHandling
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app, ICodeMap codeMap)
            => app.UseErrorHandling(codeMap.MapErrorCode);

        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app, Func<int, int> codeMap = null)
        {
            ArgumentNullException.ThrowIfNull(app);

            return app.UseErrorHandling(new ErrorHandlerOptions
            {
                CodeMap = codeMap,
            });
        }

        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app, ErrorHandlerOptions options)
        {
            ArgumentNullException.ThrowIfNull(app);
            ArgumentNullException.ThrowIfNull(options);

            return app.UseMiddleware<ErrorHandlerMiddleware>(Options.Create(options));
        }

        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app)
            => app.UseHealthCheck("/status/health-check");

        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app, string path)
        {
            ArgumentNullException.ThrowIfNull(app);

            return app.Map(path, builder =>
            {
                builder.UseMiddleware<HealthCheckMiddleware>();
            });
        }

        public static IApplicationBuilder UseReadOnlyMode(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            return app.UseMiddleware<ReadOnlyModeMiddleware>();
        }

        public static IApplicationBuilder UseVersionHeader(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            return app.UseMiddleware<VersionHeaderMiddleware>();
        }

        public static IApplicationBuilder UseVersionHeader(this IApplicationBuilder app, string header)
        {
            ArgumentNullException.ThrowIfNull(app);

            return app.UseVersionHeader(new VersionOptions
            {
                Header = header
            });
        }

        public static IApplicationBuilder UseVersionHeader(this IApplicationBuilder app, VersionOptions options)
        {
            ArgumentNullException.ThrowIfNull(app);
            ArgumentNullException.ThrowIfNull(options);

            return app.UseMiddleware<VersionHeaderMiddleware>(Options.Create(options));
        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            return app.UseMiddleware<CorrelationIdMiddleware>();
        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app, string header)
        {
            ArgumentNullException.ThrowIfNull(app);

            return app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = header
            });
        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app, CorrelationIdOptions options)
        {
            ArgumentNullException.ThrowIfNull(app);
            ArgumentNullException.ThrowIfNull(options);
            return app.UseMiddleware<CorrelationIdMiddleware>(Options.Create(options));
        }

        public static IApplicationBuilder UseBearerTokenBlacklist(this IApplicationBuilder app, BearerTokenBlacklistOptions options)
        {
            ArgumentNullException.ThrowIfNull(app);
            ArgumentNullException.ThrowIfNull(options);

            return app.UseMiddleware<BearerTokenBlacklistMiddleware>(Options.Create(options));
        }
    }
}