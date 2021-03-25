using TwentyTwenty.Mvc.Version;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class VersionServiceExtensions
    {
        public static IServiceCollection AddVersionProvider(this IServiceCollection services)
        {
            services.AddSingleton<IVersionProvider, VersionProvider>();

            return services;
        }
    }
}