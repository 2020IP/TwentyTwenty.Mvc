# TwentyTwenty.Mvc

[![Build status](https://ci.appveyor.com/api/projects/status/povg4igyov87f35b/branch/master?svg=true)](https://ci.appveyor.com/project/2020IP/twentytwenty-mvc/branch/master)
## Client IP behind the load balancer

Use the trusted forwarded headers setup so `HttpContext.Connection.RemoteIpAddress` is the client's IP, not the load balancer's, and no client can choose its own:

```csharp
// ConfigureServices
services.AddTrustedForwardedHeaders(Configuration);

// Configure: first in the pipeline, before anything reads RemoteIpAddress
app.UseForwardedHeaders();
```

`UseForwardedHeaders()` must come before anything that reads `RemoteIpAddress`, including request logging. From there on, `RemoteIpAddress` is the client IP.

Only the app's own proxies are trusted: the load balancer and anything else on a private network. There is deliberately no setting to trust more: one broad range would let clients choose their IP again. Configure it in the `ForwardedHeaders` section:

| Setting | Default | Meaning |
|---|---|---|
| `ClientIpHeader` | `X-Forwarded-For` | The header the client IP is read from, believed only when the load balancer passes it on. |

`ClientIpHeader` by setup:

| Setup | Value |
|---|---|
| No edge in front, or the Cloudflare proxy off | `X-Forwarded-For` (the default) |
| Behind Cloudflare, load balancer accepts Cloudflare only | `CF-Connecting-IP` |
| Another edge provider with its own header | Its header, e.g. `True-Client-IP` (Akamai), `X-Azure-ClientIP` (Front Door). Lock the load balancer to that provider. |

An edge header such as `CF-Connecting-IP` is only safe when the load balancer accepts traffic from that edge alone; otherwise a direct caller can send it and choose their IP. With `X-Forwarded-For`, the client IP is the address the load balancer appended, and anything a client wrote further left is ignored. Don't use `X-Forwarded-For` while traffic comes through Cloudflare: every user would resolve to a Cloudflare edge address.

The default is `X-Forwarded-For` because it can't make an app spoofable. An app behind Cloudflare that forgets to set `CF-Connecting-IP` sees Cloudflare edge addresses, which is obvious in its logs, and clients still can't choose their IP. Defaulting to `CF-Connecting-IP` would instead let any client choose its IP in an app that isn't behind a Cloudflare-only load balancer. Apps behind Cloudflare set it explicitly:

```json
"ForwardedHeaders": {
  "ClientIpHeader": "CF-Connecting-IP"
}
```

Running locally needs no change. Requests from loopback keep `127.0.0.1`, and you can simulate a client IP by sending the configured header (`X-Forwarded-For` by default).

To use `CF-Connecting-IP`, restrict the load balancer's inbound rules to [Cloudflare's IP ranges](https://www.cloudflare.com/ips/). If you ever turn the Cloudflare proxy off, open the load balancer and switch `ClientIpHeader` to `X-Forwarded-For` together. Between the two changes, users can briefly share edge addresses, or direct callers can briefly choose their IP, depending on the order.

### Keeping the raw X-Forwarded-For

`UseForwardedHeaders()` removes the entries it consumes from `X-Forwarded-For` when that is the configured header. To keep the header exactly as it arrived, for logging or investigation, capture it first:

```csharp
app.UseRawForwardedForCapture();
app.UseForwardedHeaders();
```

Then read it anywhere in the request with `RawForwardedFor.Get(HttpContext)` (an empty string if there was none). The value is kept in `HttpContext.Items`, so a client can't plant it through a request header. It is still client input: the left-hand entries are whatever the client sent. Use `RemoteIpAddress` for the client IP and this only as a record.

With an edge header such as `CF-Connecting-IP` configured, `X-Forwarded-For` isn't touched and can also be read from `Request.Headers` directly. The capture gives the same value whichever header is configured.

### Upgrading from 2.x

3.0 removes `HttpContext.SetRemoteIpAddressToForwardedFor()`. It set the remote IP to the first `X-Forwarded-For` entry, which the client writes, so any client could choose the IP an app logged or recorded. To upgrade:

1. Delete the `context.SetRemoteIpAddressToForwardedFor()` call (usually in a `Startup.Configure` middleware that also pushes `IPAddress` into the Serilog log context).
2. Add `services.AddTrustedForwardedHeaders(Configuration)` to `ConfigureServices`.
3. Add `app.UseForwardedHeaders()` at the top of `Configure`, before that middleware.
4. Set `ForwardedHeaders:ClientIpHeader` to `CF-Connecting-IP` if the app is behind Cloudflare.
5. After deploying, check the logs show real client IPs, not the load balancer's or Cloudflare's.

The old helper also read a nonstandard `Http-X-Forwarded-For` header. Nothing replaces that; common proxies and load balancers don't send it.
