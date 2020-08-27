using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace TwentyTwenty.Mvc.Version
{
    public class VersionHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IVersionProvider _versionProvider;
        private readonly VersionOptions _options;

        public VersionHeaderMiddleware(RequestDelegate next, IVersionProvider versionProvider, IOptions<VersionOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next ?? throw new ArgumentNullException(nameof(next));
            _versionProvider = versionProvider ?? throw new ArgumentNullException(nameof(versionProvider));

            _options = options.Value;
        }

        public Task Invoke(HttpContext context)
        {
            var version = _versionProvider.GetVersion();

            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(_options.Header))
                {
                    context.Response.Headers.Add(_options.Header, new[] { version });
                }
                
                return Task.CompletedTask;
            });

            return _next(context);
        }
    }
}