using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TwentyTwenty.Mvc.ForwardedHeaders;
using Xunit;

namespace TwentyTwenty.Mvc.Tests;

public class RawForwardedForTests
{
    private const string LoadBalancer = "10.0.3.17";
    private const string Client = "47.85.163.8";
    private const string Spoofed = "203.0.113.7";

    private static async Task<(string Raw, string? RemoteIp, string RemainingHeader)> Run(string? forwardedFor, Action<HttpContext>? arrange = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        services.AddTrustedForwardedHeaders(_ => { });
        var app = new ApplicationBuilder(services.BuildServiceProvider());

        string raw = null!;
        string? remoteIp = null;
        string remaining = null!;
        app.UseRawForwardedForCapture();
        app.UseForwardedHeaders();
        app.Run(ctx =>
        {
            raw = RawForwardedFor.Get(ctx);
            remoteIp = ctx.Connection.RemoteIpAddress?.ToString();
            remaining = ctx.Request.Headers["X-Forwarded-For"].ToString();
            return Task.CompletedTask;
        });

        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse(LoadBalancer);
        if (forwardedFor is not null) context.Request.Headers["X-Forwarded-For"] = forwardedFor;
        arrange?.Invoke(context);

        await app.Build()(context);
        return (raw, remoteIp, remaining);
    }

    [Fact]
    public async Task KeepsTheHeaderTheForwardedHeadersMiddlewareRewrites()
    {
        var (raw, remoteIp, remaining) = await Run($"{Spoofed}, {Client}");

        Assert.Equal(Client, remoteIp);
        Assert.Equal(Spoofed, remaining);
        Assert.Equal($"{Spoofed}, {Client}", raw);
    }

    [Fact]
    public async Task IsEmptyWithoutTheHeader()
    {
        var (raw, _, _) = await Run(null);

        Assert.Equal(string.Empty, raw);
    }

    [Fact]
    public async Task CannotBePlantedThroughARequestHeader()
    {
        var (raw, _, _) = await Run(Client, ctx => ctx.Request.Headers["X-Raw-Forwarded-For"] = Spoofed);

        Assert.Equal(Client, raw);
    }

    [Fact]
    public void IsEmptyWhenNothingWasCaptured()
    {
        Assert.Equal(string.Empty, RawForwardedFor.Get(new DefaultHttpContext()));
        Assert.Equal(string.Empty, RawForwardedFor.Get(null!));
    }
}
