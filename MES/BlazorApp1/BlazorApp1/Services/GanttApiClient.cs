using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BlazorApp1.Models;

namespace BlazorApp1.Services;

public interface IGanttApiClient
{
    Task<ResourceGanttSnapshot> GetResourceSnapshotAsync(
        long planId,
        DateTime? from,
        DateTime? to,
        bool includePersons,
        bool includeTools,
        bool includeVendors,
        string linksMode,
        string? timezone,
        CancellationToken cancellationToken);

    Task<SyncfusionResourceViewResponseVm> GetSyncfusionResourceViewAsync(
        long planId,
        DateTime? start,
        DateTime? end,
        CancellationToken cancellationToken);
}

public class GanttApiClient : IGanttApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GanttApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ResourceGanttSnapshot> GetResourceSnapshotAsync(
        long planId,
        DateTime? from,
        DateTime? to,
        bool includePersons,
        bool includeTools,
        bool includeVendors,
        string linksMode,
        string? timezone,
        CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        var query = new List<string>
        {
            $"planId={planId}",
            $"includePersons={includePersons.ToString().ToLowerInvariant()}",
            $"includeTools={includeTools.ToString().ToLowerInvariant()}",
            $"includeVendors={includeVendors.ToString().ToLowerInvariant()}",
            $"linksMode={Uri.EscapeDataString(linksMode ?? "seq")}"
        };
        if (from.HasValue)
        {
            query.Add($"from={Uri.EscapeDataString(from.Value.ToString("o"))}");
        }
        if (to.HasValue)
        {
            query.Add($"to={Uri.EscapeDataString(to.Value.ToString("o"))}");
        }
        if (!string.IsNullOrWhiteSpace(timezone))
        {
            query.Add($"timezone={Uri.EscapeDataString(timezone)}");
        }

        var url = $"api/gantt/resource-snapshot?{string.Join("&", query)}";
        using var response = await _http.GetAsync(url, cancellationToken);
        var body = response.Content == null ? string.Empty : await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Gantt snapshot request failed: {(int)response.StatusCode}. {body}");
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return new ResourceGanttSnapshot();
        }

        return JsonSerializer.Deserialize<ResourceGanttSnapshot>(body, _jsonOptions) ?? new ResourceGanttSnapshot();
    }



    public async Task<SyncfusionResourceViewResponseVm> GetSyncfusionResourceViewAsync(
        long planId,
        DateTime? start,
        DateTime? end,
        CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        var query = new List<string>
        {
            $"planId={planId}"
        };
        if (start.HasValue)
        {
            query.Add($"start={Uri.EscapeDataString(start.Value.ToString("o"))}");
        }
        if (end.HasValue)
        {
            query.Add($"end={Uri.EscapeDataString(end.Value.ToString("o"))}");
        }

        var url = $"api/gantt/syncfusion/resource-view?{string.Join("&", query)}";
        using var response = await _http.GetAsync(url, cancellationToken);
        var body = response.Content == null ? string.Empty : await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Syncfusion gantt request failed: {(int)response.StatusCode}. {body}");
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return new SyncfusionResourceViewResponseVm();
        }

        return JsonSerializer.Deserialize<SyncfusionResourceViewResponseVm>(body, _jsonOptions) ?? new SyncfusionResourceViewResponseVm();
    }

    private void EnsureBaseAddress()
    {
        if (_http.BaseAddress == null)
        {
            throw new InvalidOperationException("ApsApi BaseUrl is not configured.");
        }
    }
}
