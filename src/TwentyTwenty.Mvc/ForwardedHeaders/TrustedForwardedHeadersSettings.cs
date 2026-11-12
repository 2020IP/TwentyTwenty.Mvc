namespace TwentyTwenty.Mvc.ForwardedHeaders
{
    /// <summary>
    /// How the client IP is found behind a load balancer. Bound from the <c>ForwardedHeaders</c> section
    /// by <c>AddTrustedForwardedHeaders</c>.
    /// </summary>
    public class TrustedForwardedHeadersSettings
    {
        public const string SectionName = "ForwardedHeaders";

        /// <summary>
        /// The header the client IP is read from, believed only when a trusted proxy (the load balancer)
        /// passes it on.
        /// <list type="bullet">
        /// <item><c>X-Forwarded-For</c> (default): traffic reaches the load balancer directly, for example with
        /// no edge in front or the Cloudflare proxy off. The client IP is the address the load balancer
        /// appended; entries a client wrote further left are ignored.</item>
        /// <item><c>CF-Connecting-IP</c>: behind Cloudflare. Cloudflare always overwrites it, and is only safe
        /// when the load balancer accepts traffic from Cloudflare alone. Other edge providers have their own
        /// equivalent, such as <c>True-Client-IP</c> (Akamai) or <c>X-Azure-ClientIP</c> (Front Door).</item>
        /// </list>
        /// The default never lets a client choose its IP: an app behind Cloudflare that leaves it unset sees
        /// Cloudflare edge addresses instead, which shows up plainly in its logs. An edge header without an
        /// edge-only load balancer lets any client choose its IP by sending it. Empty means
        /// <c>X-Forwarded-For</c>.
        /// </summary>
        public string ClientIpHeader { get; set; } = Microsoft.AspNetCore.HttpOverrides.ForwardedHeadersDefaults.XForwardedForHeaderName;
    }
}
