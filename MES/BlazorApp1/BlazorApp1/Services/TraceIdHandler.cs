using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BlazorApp1.Services;

public sealed class TraceIdHandler : DelegatingHandler
{
    private const string TraceHeader = "X-Trace-Id";
    private readonly ILogger<TraceIdHandler> _logger;

    public TraceIdHandler(ILogger<TraceIdHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var traceId = TraceContext.TraceId;
        if (string.IsNullOrWhiteSpace(traceId))
        {
            traceId = Guid.NewGuid().ToString("N");
            TraceContext.TraceId = traceId;
        }

        request.Headers.Remove(TraceHeader);
        request.Headers.Add(TraceHeader, traceId);

        var url = request.RequestUri?.ToString() ?? "(unknown)";
        _logger.LogInformation("APS API request start {Method} {Url} TraceId={TraceId}", request.Method, url, traceId);

        var sw = Stopwatch.StartNew();
        var response = await base.SendAsync(request, cancellationToken);
        sw.Stop();

        _logger.LogInformation(
            "APS API request end {StatusCode} {Method} {Url} {ElapsedMs}ms TraceId={TraceId}",
            (int)response.StatusCode,
            request.Method,
            url,
            sw.ElapsedMilliseconds,
            traceId);

        return response;
    }
}
