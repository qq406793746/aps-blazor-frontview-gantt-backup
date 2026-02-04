using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorApp1.Models;

public class SyncfusionResourceViewResponseVm
{
    public SyncfusionMetaVm Meta { get; set; } = new();
    public List<GanttResourceVm> Resources { get; set; } = new();
    public List<GanttTaskVm> Tasks { get; set; } = new();
    public List<GanttLinkVm> Links { get; set; } = new();
}

public class SyncfusionMetaVm
{
    public long PlanId { get; set; }
    public string PlanNo { get; set; } = string.Empty;
    public DateTime? PlanningStartTime { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class GanttResourceVm
{
    [JsonPropertyName("resourceId")]
    public int Id { get; set; }

    [JsonPropertyName("resourceName")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("resourceCode")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("resourceType")]
    public string Type { get; set; } = string.Empty;

    [JsonIgnore]
    public int MaxUnits { get; set; } = 100;
}

public class GanttTaskVm
{
    public int TaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    [JsonIgnore]
    public double DurationValue { get; set; }
    [JsonIgnore]
    public string DurationUnit { get; set; } = "minute";
    [JsonPropertyName("duration")]
    public double? RawDurationMinutes { get; set; }
    public List<int> ResourceIds { get; set; } = new();
    public List<int> ResourceInfo
    {
        get => ResourceIds;
        set => ResourceIds = value ?? new List<int>();
    }
    public bool IsExpanded { get; set; } = true;
    public string? Predecessor { get; set; }
    public string CssClass { get; set; } = string.Empty;
    public bool IsOverdue { get; set; }
    public bool IsAnomaly { get; set; }
    public string? OrderNo { get; set; }
    public string? OperationName { get; set; }
    public int? OpSeq { get; set; }
    public DateTime? DueDate { get; set; }
}

public class GanttLinkVm
{
    public int FromTaskId { get; set; }
    public int ToTaskId { get; set; }
    public string Type { get; set; } = "FS";
}

public class GanttAssignmentVm
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int ResourceId { get; set; }
    public int Unit { get; set; } = 100;
}
