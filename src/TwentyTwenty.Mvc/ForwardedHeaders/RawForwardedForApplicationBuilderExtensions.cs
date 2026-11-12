using System;
using TwentyTwenty.Mvc.ForwardedHeaders;

namespace Microsoft.AspNetCore.Builder
{
    public static class RawForwardedForApplicationBuilderExtensions
    {
        /// <summary>
        /// Keeps the <c>X-Forwarded-For</c> header as received, readable with
        /// <see cref="RawForwardedFor.Get"/>. Register it before <c>app.UseForwardedHeaders()</c>.
        /// </summary>
        public static IApplicationBuilder UseRawForwardedForCapture(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            return app.Use((context, next) =>
            {
                RawForwardedFor.Capture(context);
                return next(context);
            });
        }
    }
}
