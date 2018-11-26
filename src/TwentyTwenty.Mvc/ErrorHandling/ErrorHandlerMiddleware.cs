using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using TwentyTwenty.BaseLine;

namespace TwentyTwenty.Mvc.ErrorHandling
{
    public class ErrorHandlerMiddleware
    {
        private const string JsonContentType = "application/json";
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _env;
        private readonly Func<object, Task> _clearCacheHeadersDelegate;
        private readonly ErrorHandlerOptions _options;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger, IHostingEnvironment env, IOptions<ErrorHandlerOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next;
            _logger = logger;
            _env = env;
            _options = options.Value;

            _clearCacheHeadersDelegate = ClearCacheHeaders;
        }

        public async Task Invoke(HttpContext context)
        {
            bool isAjaxRequest = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                || context.Request.Headers[HeaderNames.Accept] == JsonContentType
                || context.Request.ContentType == JsonContentType;
            
            try
            {
                if (_env.IsDevelopment() && context.Request.Query.ContainsKey("throw"))
                {
                    throw new Exception("Generic test error.");
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "An unhandled exception has occurred: " + ex.Message);
                // We can't do anything if the response has already started, just abort.
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the error handler will not be executed.");
                    throw;
                }
                try
                {
                    context.Response.StatusCode = 500;
                    context.Response.OnStarting(_clearCacheHeadersDelegate, context.Response);

                    var errorResponse = new ErrorResponse
                    {
                        RequestId = context.TraceIdentifier,
                        ErrorMessage = ex.Message,
                    };

                    for (var ttEx = ex as TwentyTwentyException; ttEx != null; ttEx = null)
                    {
                        errorResponse.ErrorCode = ttEx.ErrorCode;
                        context.Response.StatusCode = _options.CodeMap?.Invoke(ttEx.ErrorCode) ?? 500;
                    }

                    if (!_options.UseHtmlPage || isAjaxRequest)
                    {
                        context.Response.ContentType = JsonContentType;

                        if (_env.IsDevelopment())
                        {
                            errorResponse.Details = GetDetails(ex, context);
                        }

                        var json = JsonConvert.SerializeObject(errorResponse);
                        await context.Response.WriteAsync(json);
                    }
                    else
                    {
                        // TODO: Html error page.
                        await context.Response.WriteAsync("Error.");
                    }
                    return;
                }
                catch (Exception ex2)
                {
                    // If there's a Exception while generating the error page, re-throw the original exception.
                    _logger.LogError(0, ex2, "An exception was thrown attempting to display the error page.");
                }
                // Re-throw the original exception if we can't handle it.
                throw;
            }
        }

        private ErrorDetails GetDetails(Exception ex, HttpContext context)
        {
            return new ErrorDetails
            {
                StackTrace = ex.StackTrace,
                RequestPath = context.Request.Path,
                QueryString = context.Request.QueryString.ToString(),
            };
        }

        private Task ClearCacheHeaders(object state)
        {
            var response = (HttpResponse)state;
            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";
            response.Headers.Remove(HeaderNames.ETag);
            return Task.FromResult(true);
        }
    }
}