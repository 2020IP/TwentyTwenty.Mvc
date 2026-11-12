using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TwentyTwenty.Mvc.ForwardedHeaders;
using Xunit;

namespace TwentyTwenty.Mvc.Tests;

/// <summary>
/// A client must not be able to choose the IP the app sees. Only the app's own proxies are believed: behind
/// Cloudflare the IP comes from CF-Connecting-IP, otherwise from the entry the load balancer appended.
/// </summary>
public class TrustedForwardedHeadersTests
{
    private const string LoadBalancer = "10.0.3.17";
    private const string CloudflareEdge = "172.70.1.2";
    private const string Client = "47.85.163.8";
    private const string Spoofed = "203.0.113.7";

    private static ForwardedHeadersOptions BuildOptions(Action<TrustedForwardedHeadersSettings> configure)
    {
        var services = new ServiceCollection();
        services.AddTrustedForwardedHeaders(configure);
        return services.BuildServiceProvider().GetRequiredService<IOptions<ForwardedHeadersOptions>>().Value;
    }

    private static async Task<string?> Resolve(
        ForwardedHeadersOptions options, string connectionIp, string? forwardedFor = null, string? cfConnectingIp = null)
    {
        string? resolved = null;
        var middleware = new ForwardedHeadersMiddleware(
            ctx => { resolved = ctx.Connection.RemoteIpAddress?.ToString(); return Task.CompletedTask; },
            NullLoggerFactory.Instance,
            Options.Create(options));

        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse(connectionIp);
        if (forwardedFor is not null) context.Request.Headers["X-Forwarded-For"] = forwardedFor;
        if (cfConnectingIp is not null) context.Request.Headers["CF-Connecting-IP"] = cfConnectingIp;

        await middleware.Invoke(context);
        return resolved;
    }

    private static Task<string?> BehindCloudflare(string connectionIp, string? forwardedFor = null, string? cfConnectingIp = null) =>
        Resolve(BuildOptions(s => s.ClientIpHeader = "CF-Connecting-IP"), connectionIp, forwardedFor, cfConnectingIp);

    private static Task<string?> Direct(string connectionIp, string? forwardedFor = null, string? cfConnectingIp = null) =>
        Resolve(BuildOptions(s => s.ClientIpHeader = "X-Forwarded-For"), connectionIp, forwardedFor, cfConnectingIp);

    [Fact]
    public async Task BehindCloudflare_ResolvesCfConnectingIp_IgnoringXForwardedFor()
    {
        Assert.Equal(Client, await BehindCloudflare(LoadBalancer, $"{Spoofed}, {Client}, {CloudflareEdge}", Client));
    }

    [Fact]
    public async Task BehindCloudflare_DoesNotBelieveAPublicPeer()
    {
        Assert.Equal(Client, await BehindCloudflare(Client, cfConnectingIp: Spoofed));
    }

    [Fact]
    public async Task BehindCloudflare_WithoutTheHeader_KeepsTheConnectionAddress()
    {
        Assert.Equal(LoadBalancer, await BehindCloudflare(LoadBalancer));
        Assert.Equal("127.0.0.1", await BehindCloudflare("127.0.0.1"));
    }

    [Fact]
    public async Task BehindCloudflare_TrustsAnIpv4MappedLoadBalancer()
    {
        Assert.Equal(Client, await BehindCloudflare("::ffff:10.0.3.17", cfConnectingIp: Client));
    }

    [Fact]
    public async Task Direct_ResolvesTheAddressTheLoadBalancerAppended()
    {
        Assert.Equal(Client, await Direct(LoadBalancer, $"1.1.1.1, {Spoofed}, {Client}"));
    }

    [Fact]
    public async Task Direct_IgnoresCfConnectingIpSentByTheClient()
    {
        Assert.Equal(Client, await Direct(LoadBalancer, Client, Spoofed));
    }

    [Fact]
    public async Task Direct_DoesNotTrustAPublicProxy()
    {
        Assert.Equal("198.51.100.9", await Direct(LoadBalancer, $"{Client}, 198.51.100.9"));
    }

    [Fact]
    public async Task AnotherEdgeHeader_IsReadTheSameWay()
    {
        var options = BuildOptions(s => s.ClientIpHeader = "True-Client-IP");
        var middlewareResult = await ResolveWith(options, LoadBalancer, "True-Client-IP", Client);

        Assert.Equal(Client, middlewareResult);
        Assert.Equal(LoadBalancer, await ResolveWith(options, LoadBalancer, "CF-Connecting-IP", Spoofed));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void EmptyHeader_MeansXForwardedFor(string? header)
    {
        Assert.Equal("X-Forwarded-For", BuildOptions(s => s.ClientIpHeader = header!).ForwardedForHeaderName);
    }

    private static async Task<string?> ResolveWith(ForwardedHeadersOptions options, string connectionIp, string header, string value)
    {
        string? resolved = null;
        var middleware = new ForwardedHeadersMiddleware(
            ctx => { resolved = ctx.Connection.RemoteIpAddress?.ToString(); return Task.CompletedTask; },
            NullLoggerFactory.Instance,
            Options.Create(options));

        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse(connectionIp);
        context.Request.Headers[header] = value;

        await middleware.Invoke(context);
        return resolved;
    }

    [Fact]
    public async Task Default_IsXForwardedFor_SoMissingConfigNeverLetsAClientChooseItsIp()
    {
        var options = BuildOptions(_ => { });

        Assert.Equal("X-Forwarded-For", options.ForwardedForHeaderName);
        // Not behind Cloudflare: correct, and a client-sent CF-Connecting-IP is ignored.
        Assert.Equal(Client, await Resolve(options, LoadBalancer, $"{Spoofed}, {Client}", Spoofed));
        // Behind Cloudflare but not configured: the edge address, never a client-chosen one.
        Assert.Equal(CloudflareEdge, await Resolve(options, LoadBalancer, $"{Spoofed}, {Client}, {CloudflareEdge}", Client));
    }

    [Fact]
    public void BindsFromTheForwardedHeadersSection()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ForwardedHeaders:ClientIpHeader"] = "CF-Connecting-IP",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddTrustedForwardedHeaders(configuration);
        var options = services.BuildServiceProvider().GetRequiredService<IOptions<ForwardedHeadersOptions>>().Value;

        Assert.Equal("CF-Connecting-IP", options.ForwardedForHeaderName);
        Assert.Equal(
            TrustedForwardedHeadersConfigureOptions.PrivateNetworks,
            options.KnownIPNetworks.Select(n => n.ToString()));
    }
}
