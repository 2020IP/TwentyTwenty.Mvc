using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TwentyTwenty.Mvc.Version
{
    public class VersionHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IVersionProvider _versionProvider;
        private readonly string _headerName;

        public VersionHeaderMiddleware(RequestDelegate next, IVersionProvider versionProvider, string headerName)
        {
            _next = next;
            _versionProvider = versionProvider;
            _headerName = headerName;
        }

        public async Task Invoke(HttpContext context)
        {
            var headerName = string.IsNullOrWhiteSpace(_headerName) ? "api-version" : _headerName;
            var version = _versionProvider.GetVersion();

            await _next(context);

            context.Response.Headers.Add(headerName, version);
        }
    }
}