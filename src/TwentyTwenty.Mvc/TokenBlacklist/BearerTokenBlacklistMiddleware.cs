using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace TwentyTwenty.Mvc.TokenBlacklist
{
    public class BearerTokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;
        private readonly BearerTokenBlacklistOptions _options;
        
        public BearerTokenBlacklistMiddleware(RequestDelegate next, IDistributedCache cache, IOptions<BearerTokenBlacklistOptions> options)
        {
            _next = next;
            _cache = cache;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out StringValues bearer))
            {
                var token = bearer.ToString().Replace("Bearer", string.Empty).Trim();
                var value = await _cache.GetAsync(string.Format(_options.CacheKeyFormatString, token));

                if (value != null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }

            await _next(context);
        }
    }
}