using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using IPNetwork = System.Net.IPNetwork;

namespace TwentyTwenty.Mvc.ForwardedHeaders
{
    /// <summary>
    /// Decides which forwarded client IP to believe. Only the app's own proxies are trusted: the load balancer
    /// and anything else on a private network. The IP is read from the configured client IP header, such as
    /// <c>CF-Connecting-IP</c> behind Cloudflare; with <c>X-Forwarded-For</c> it is the address the load
    /// balancer appended, and entries a client wrote further left are never reached. Unlike taking the first <c>X-Forwarded-For</c> entry,
    /// a client cannot choose its own IP.
    /// </summary>
    public class TrustedForwardedHeadersConfigureOptions : IConfigureOptions<ForwardedHeadersOptions>
    {
        public const string CloudflareConnectingIpHeader = "CF-Connecting-IP";

        /// <summary>
        /// Addresses load balancers and ingress inside a private network use; no public client can connect from them.
        /// 100.64.0.0/10 is carrier-grade NAT space, which some cloud load balancers use internally.
        /// </summary>
        public static readonly IReadOnlyList<string> PrivateNetworks = new[]
        {
            "10.0.0.0/8",
            "172.16.0.0/12",
            "192.168.0.0/16",
            "100.64.0.0/10",
            "127.0.0.0/8",
            "::1/128",
            "fc00::/7",
        };

        private readonly IOptions<TrustedForwardedHeadersSettings> _settings;

        public TrustedForwardedHeadersConfigureOptions(IOptions<TrustedForwardedHeadersSettings> settings)
        {
            _settings = settings;
        }

        public void Configure(ForwardedHeadersOptions options)
        {
            var settings = _settings.Value;

            options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor;
            options.ForwardedForHeaderName = string.IsNullOrWhiteSpace(settings.ClientIpHeader)
                ? ForwardedHeadersDefaults.XForwardedForHeaderName
                : settings.ClientIpHeader.Trim();

            // Walk back through as many trusted proxies as there are and stop at the first address that is
            // not one. Safe only because the trusted networks are limited to private ranges.
            options.ForwardLimit = null;

            options.KnownProxies.Clear();
            options.KnownIPNetworks.Clear();
            foreach (var cidr in PrivateNetworks)
            {
                options.KnownIPNetworks.Add(IPNetwork.Parse(cidr.Trim()));
            }
        }
    }
}
