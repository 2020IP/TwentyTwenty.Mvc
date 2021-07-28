using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace TwentyTwenty.Mvc
{
    public static class HttpContextExtensions
    {
        public static void SetRemoteIpAddressToForwardedFor(this HttpContext httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues forwardValue))
            {
                SetRemoveIpAddress(forwardValue, httpContext);
            }
            else if (httpContext.Request.Headers.TryGetValue("Http-X-Forwarded-For", out StringValues httpForwardValue))
            {
                SetRemoveIpAddress(httpForwardValue, httpContext);
            }
        }

        private static void SetRemoveIpAddress(StringValues value, HttpContext httpContext)
        {
            var forward = value[0].Split(new[] { ", ", "," }, StringSplitOptions.None)[0];

            if (IPAddress.TryParse(forward, out IPAddress remoteIp))
            {
                httpContext.Connection.RemoteIpAddress = remoteIp;
            }
        }
    }
}