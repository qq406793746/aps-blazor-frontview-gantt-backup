using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorApp1.Services;

public class ApsApiOptions
{
    public string? BaseUrl { get; set; }
}

public interface IApsApiService
{
    Task<IReadOnlyList<GanttTaskDto>> GetTasksAsync(long planId, CancellationToken cancellationToken);
    Task<MoveScheduleTaskResult> MoveTaskAsync(long taskId, MoveScheduleTaskRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<ScheduleChangeLogItemDto>> GetChangesAsync(long taskId, CancellationToken cancellationToken);
    Task<PlanAutoScheduleResult> AutoScheduleAsync(long planId, string engine, bool force, PlanAutoScheduleRequest request, CancellationToken cancellationToken);
    Task<PagedResult<ProjectListItemDto>> GetProjectsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<WorkItemListItemDto>> GetWorkItemsAsync(long projectId, int pageIndex, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<PlanListItemDto>> GetPlansAsync(long projectId, int pageIndex, int pageSize, CancellationToken cancellationToken);
    Task<PlanListItemDto?> GetLatestPlanAsync(CancellationToken cancellationToken);
    Task<PagedResult<ProcessRouteListItemDto>> GetRoutesAsync(long projectId, int pageIndex, int pageSize, CancellationToken cancellationToken);
    Task<long> CreateWorkItemAsync(WorkItemCreateRequest request, CancellationToken cancellationToken);
    Task<long> CreatePlanAsync(PlanCreateRequest request, CancellationToken cancellationToken);
    Task GenerateTasksAsync(long planId, PlanGenerateTasksRequest request, CancellationToken cancellationToken);
}

public class ApsApiService : IApsApiService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApsApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<GanttTaskDto>> GetTasksAsync(long planId, CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        using var response = await _http.GetAsync($"api/plans/{planId}/tasks", cancellationToken);
        return await ReadAsync<List<GanttTaskDto>>(response, cancellationToken)
               ?? new List<GanttTaskDto>();
    }

    public async Task<MoveScheduleTaskResult> MoveTaskAsync(long taskId, MoveScheduleTaskRequest request, CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        using var message = new HttpRequestMessage(new HttpMethod("PATCH"), $"api/schedule/tasks/{taskId}/move")
        {
            Content = JsonContent.Create(request)
        };
        using var response = await _http.SendAsync(message, cancellationToken);
        return await ReadAsync<MoveScheduleTaskResult>(response, cancellationToken)
               ?? new MoveScheduleTaskResult();
    }

    public async Task<IReadOnlyList<ScheduleChangeLogItemDto>> GetChangesAsync(long taskId, CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        using var response = await _http.GetAsync($"api/schedule/tasks/{taskId}/changes", cancellationToken);
        return await ReadAsync<List<ScheduleChangeLogItemDto>>(response, cancellationToken)
               ?? new List<ScheduleChangeLogItemDto>();
    }

    public async Task<PlanAutoScheduleResult> AutoScheduleAsync(long planId, string engine, bool force, PlanAutoScheduleRequest request, CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        var query = $"api/plans/{planId}/auto-schedule?engine={Uri.EscapeDataString(engine)}&force={force.ToString().ToLowerInvariant()}";
        using var response = await _http.PostAsJsonAsync(query, request, cancellationToken);
        return await ReadAsync<PlanAutoScheduleResult>(response, cancellationToken)
               ?? new PlanAutoScheduleResult();
    }

    public async Task<PagedResult<ProjectListItemDto>> GetProjectsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        var url = $"api/projects?pageIndex={pageIndex}&pageSize={pageSize}";
        using var response = await _http.GetAsync(url, cancellationToken);
        return await ReadAsync<PagedResult<ProjectListItemDto>>(response, cancellationToken)
               ?? new PagedResult<ProjectListItemDto>();
    }

    public async Task<PagedResult<WorkItemListItemDto>> GetWorkItemsAsync(long projectId, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        var url = $"api/work-items?pageIndex={pageIndex}&pageSize={pageSize}&projectId={projectId}";
        using var response = await _http.GetAsync(url, cancellationToken);
        return await ReadAsync<PagedResult<WorkItemListItemDto>>(response, cancellationToken)
               ?? new PagedResult<WorkItemListItemDto>();
    }

    public async Task<PagedResult<PlanListItemDto>> GetPlansAsync(long projectId, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        var url = $"api/plans?pageIndex={pageIndex}&pageSize={pageSize}&projectId={projectId}";
        using var response = await _http.GetAsync(url, cancellationToken);
        return await ReadAsync<PagedResult<PlanListItemDto>>(response, cancellationToken)
               ?? new PagedResult<PlanListItemDto>();
    }

    public async Task<PlanListItemDto?> GetLatestPlanAsync(CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        var url = "api/plans?pageIndex=1&pageSize=1";
        using var response = await _http.GetAsync(url, cancellationToken);
        var page = await ReadAsync<PagedResult<PlanListItemDto>>(response, cancellationToken)
                   ?? new PagedResult<PlanListItemDto>();
        return page.Items.FirstOrDefault();
    }

    public async Task<PagedResult<ProcessRouteListItemDto>> GetRoutesAsync(long projectId, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        var url = $"api/process-routes?pageIndex={pageIndex}&pageSize={pageSize}&projectId={projectId}";
        using var response = await _http.GetAsync(url, cancellationToken);
        return await ReadAsync<PagedResult<ProcessRouteListItemDto>>(response, cancellationToken)
               ?? new PagedResult<ProcessRouteListItemDto>();
    }

    public async Task<long> CreateWorkItemAsync(WorkItemCreateRequest request, CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        using var response = await _http.PostAsJsonAsync("api/work-items", request, cancellationToken);
        var created = await ReadAsync<CreatedId>(response, cancellationToken)
                      ?? new CreatedId();
        return created.Id;
    }

    public async Task<long> CreatePlanAsync(PlanCreateRequest request, CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        using var response = await _http.PostAsJsonAsync("api/plans", request, cancellationToken);
        var created = await ReadAsync<PlanCreatedDto>(response, cancellationToken)
                      ?? new PlanCreatedDto();
        return created.Id;
    }

    public async Task GenerateTasksAsync(long planId, PlanGenerateTasksRequest request, CancellationToken cancellationToken)
    {
        EnsureBaseAddress();
        using var response = await _http.PostAsJsonAsync($"api/plans/{planId}/generate-tasks", request, cancellationToken);
        await ReadAsync<object>(response, cancellationToken);
    }

    private void EnsureBaseAddress()
    {
        if (_http.BaseAddress == null)
        {
            throw new InvalidOperationException("ApsApi BaseUrl is not configured.");
        }
    }

    private async Task<T?> ReadAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var body = response.Content == null ? string.Empty : await response.Content.ReadAsStringAsync(cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(body, _jsonOptions);
        }

        ApiError? error = null;
        if (!string.IsNullOrWhiteSpace(body))
        {
            try
            {
                error = JsonSerializer.Deserialize<ApiError>(body, _jsonOptions);
            }
            catch
            {
                // ignore parsing error and surface raw body
            }
        }

        throw new ApsApiException(response.StatusCode, error, body);
    }
}

public sealed class ApsApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public ApiError? Error { get; }
    public string? RawBody { get; }

    public ApsApiException(HttpStatusCode statusCode, ApiError? error, string? rawBody)
        : base(error?.Message ?? $"APS API request failed ({(int)statusCode}).")
    {
        StatusCode = statusCode;
        Error = error;
        RawBody = rawBody;
    }
}

public class ApiError
{
    public string? Code { get; set; }
    public string? Message { get; set; }
    public string? TraceId { get; set; }
}

public class GanttTaskDto
{
    public long TaskId { get; set; }
    public long PlanId { get; set; }
    public long? WorkItemId { get; set; }
    public string? OrderNo { get; set; }
    public string? OperationName { get; set; }
    public int Seq { get; set; }
    public string? MachineCode { get; set; }
    public DateTime? PlannedStart { get; set; }
    public DateTime? PlannedEnd { get; set; }
    public bool IsLocked { get; set; }
    public bool? IsUrgent { get; set; }
    public int? UrgentLevel { get; set; }
    public bool IsOutsourced { get; set; }
    public string? VendorName { get; set; }
    public DateTime? ReturnAt { get; set; }
    public int Status { get; set; }
}

public class MoveScheduleTaskRequest
{
    public DateTime? NewStart { get; set; }
    public DateTime? NewEnd { get; set; }
    public int? DeltaMinutes { get; set; }
    public bool? AllowPush { get; set; }
    public string? Reason { get; set; }
    public string? ChangedBy { get; set; }
    public string? Source { get; set; }
}

public class MoveScheduleTaskResult
{
    public long TaskId { get; set; }
    public DateTime? OldStart { get; set; }
    public DateTime? OldEnd { get; set; }
    public DateTime? NewStart { get; set; }
    public DateTime? NewEnd { get; set; }
    public int PushedCount { get; set; }
    public IReadOnlyList<long> PushedTaskIds { get; set; } = Array.Empty<long>();
}

public class ScheduleChangeLogItemDto
{
    public long Id { get; set; }
    public long TaskId { get; set; }
    public long PlanId { get; set; }
    public DateTime? OldStart { get; set; }
    public DateTime? OldEnd { get; set; }
    public DateTime? NewStart { get; set; }
    public DateTime? NewEnd { get; set; }
    public string? Reason { get; set; }
    public string? ChangedBy { get; set; }
    public string? Source { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PlanAutoScheduleRequest
{
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public IReadOnlyList<long>? UrgentWorkItemIds { get; set; }
    public int? FreezeWindowHours { get; set; }
    public bool? PreferCoreMachines { get; set; }
    public string? UrgentPolicy { get; set; }
    public bool? AllowOutsource { get; set; }
}

public class PlanAutoScheduleResult
{
    public long PlanId { get; set; }
    public string? Engine { get; set; }
    public int ScheduledCount { get; set; }
    public int UnscheduledCount { get; set; }
    public int ElapsedMs { get; set; }
    public int AffectedTaskCount { get; set; }
    public int TotalDelayMinutes { get; set; }
    public int ToolShortageCount { get; set; }
    public int ToolConflictCount { get; set; }
    public int MaterialNotReadyCount { get; set; }
    public int OutsourcedCount { get; set; }
}

public class PagedResult<T>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
}

public class ProjectListItemDto
{
    public long Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Customer { get; set; }
}

public class WorkItemListItemDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string? OrderNo { get; set; }
    public string? PartNo { get; set; }
    public string? MoldNo { get; set; }
    public decimal Qty { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? MaterialReadyAt { get; set; }
    public int Priority { get; set; }
}

public class PlanListItemDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string PlanNo { get; set; } = "";
    public DateTime DueDate { get; set; }
    public int Priority { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProcessRouteListItemDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string Name { get; set; } = "";
}

public class WorkItemCreateRequest
{
    public long ProjectId { get; set; }
    public string? OrderNo { get; set; }
    public string? PartNo { get; set; }
    public string? MoldNo { get; set; }
    public decimal Qty { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? MaterialReadyAt { get; set; }
    public int? Priority { get; set; }
}

public class PlanCreateRequest
{
    public long ProjectId { get; set; }
    public DateTime DueDate { get; set; }
    public int? Priority { get; set; }
    public int? Status { get; set; }
}

public class PlanCreatedDto
{
    public long Id { get; set; }
    public string PlanNo { get; set; } = "";
}

public class PlanGenerateTasksRequest
{
    public long? RouteId { get; set; }
    public long? WorkItemId { get; set; }
    public bool Force { get; set; }
}

public class CreatedId
{
    public long Id { get; set; }
}
