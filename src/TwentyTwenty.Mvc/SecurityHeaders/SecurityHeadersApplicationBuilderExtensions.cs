using System;
using TwentyTwenty.Mvc.SecurityHeaders;

namespace Microsoft.AspNetCore.Builder
{
    public static class SecurityHeadersApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSecurityHeadersMiddleware(this IApplicationBuilder app, SecurityHeadersBuilder builder)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return app.UseMiddleware<SecurityHeadersMiddleware>(builder.Build());
        }

        public static IApplicationBuilder UseSecurityHeadersMiddleware(this IApplicationBuilder app, Action<SecurityHeadersBuilder> builderAction)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (builderAction == null)
            {
                throw new ArgumentNullException(nameof(builderAction));
            }

            var builder = new SecurityHeadersBuilder();
            builderAction(builder);

            return app.UseMiddleware<SecurityHeadersMiddleware>(builder.Build());
        }
    }
}