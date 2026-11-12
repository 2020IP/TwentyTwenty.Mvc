using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;

namespace TwentyTwenty.Mvc.ForwardedHeaders
{
    /// <summary>
    /// The <c>X-Forwarded-For</c> header exactly as the request arrived, for logging and investigation.
    /// The forwarded headers middleware removes the entries it consumes when it reads
    /// <c>X-Forwarded-For</c>, so capture it with <c>app.UseRawForwardedForCapture()</c> before
    /// <c>app.UseForwardedHeaders()</c>. The value is kept in <see cref="HttpContext.Items"/>, where a
    /// client cannot plant it the way it could a request header.
    /// </summary>
    /// <remarks>
    /// The value is client input: the left-hand entries are whatever the client sent. Use
    /// <c>HttpContext.Connection.RemoteIpAddress</c> for the client IP, and this only as a record.
    /// </remarks>
    public static class RawForwardedFor
    {
        internal static readonly object ItemKey = new object();

        /// <summary>The <c>X-Forwarded-For</c> header as received, or an empty string.</summary>
        public static string Get(HttpContext context) =>
            context != null && context.Items.TryGetValue(ItemKey, out var value) ? value as string ?? string.Empty : string.Empty;

        internal static void Capture(HttpContext context) =>
            context.Items[ItemKey] = context.Request.Headers[ForwardedHeadersDefaults.XForwardedForHeaderName].ToString();
    }
}
