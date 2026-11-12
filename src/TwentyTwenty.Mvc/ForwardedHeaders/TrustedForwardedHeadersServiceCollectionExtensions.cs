using System;
using Microsoft.Extensions.Configuration;
using TwentyTwenty.Mvc.ForwardedHeaders;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TrustedForwardedHeadersServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the forwarded headers middleware to find the client IP safely behind a load
        /// balancer, from the <c>ForwardedHeaders</c> section of <paramref name="configuration"/>.
        /// Call <c>app.UseForwardedHeaders()</c> first in the pipeline, before anything reads
        /// <c>HttpContext.Connection.RemoteIpAddress</c>.
        /// </summary>
        public static IServiceCollection AddTrustedForwardedHeaders(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<TrustedForwardedHeadersSettings>(configuration.GetSection(TrustedForwardedHeadersSettings.SectionName));
            return services.AddTrustedForwardedHeadersCore();
        }

        /// <inheritdoc cref="AddTrustedForwardedHeaders(IServiceCollection, IConfiguration)"/>
        public static IServiceCollection AddTrustedForwardedHeaders(this IServiceCollection services, Action<TrustedForwardedHeadersSettings> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            services.Configure(configure);
            return services.AddTrustedForwardedHeadersCore();
        }

        private static IServiceCollection AddTrustedForwardedHeadersCore(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.ConfigureOptions<TrustedForwardedHeadersConfigureOptions>();
            return services;
        }
    }
}
