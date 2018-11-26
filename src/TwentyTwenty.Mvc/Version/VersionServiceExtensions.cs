using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TwentyTwenty.Mvc.DataTables.Core;
using Microsoft.AspNetCore.Mvc;
using TwentyTwenty.Mvc.DataTables;
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