using System;
using System.Collections.Generic;
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
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public int MaxUnits { get; set; } = 100;

    public string GroupName { get; set; } = string.Empty;

    public int? ParentId { get; set; }

    public int SortKey { get; set; }
}

public class GanttTaskVm
{
    public int TaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DurationValue { get; set; }
    public string DurationUnit { get; set; } = "minute";
    public double? RawDurationMinutes { get; set; }
    public List<int> ResourceIds { get; set; } = new();
    public bool IsExpanded { get; set; } = true;
    public string? Predecessor { get; set; }
    public string CssClass { get; set; } = string.Empty;
    public bool IsOverdue { get; set; }
    public bool IsAnomaly { get; set; }
    public string? OrderNo { get; set; }
    public string? PartNo { get; set; }
    public string? MoldNo { get; set; }
    public string? OperationName { get; set; }
    public int? OpSeq { get; set; }
    public DateTime? DueDate { get; set; }
    public int SetupMinutes { get; set; }
    public int? RunMinutes { get; set; }
    public int Priority { get; set; }
    public bool IsUrgent { get; set; }
    public bool IsLocked { get; set; }
    public bool IsOutsourced { get; set; }
    public bool HasPrecedenceViolation { get; set; }
    public bool HasResourceConflict { get; set; }
    public bool HasMaterialNotReady { get; set; }
    public string? AnomalyReason { get; set; }
    public string Status { get; set; } = "Planned";
    public DateTime? ActualStart { get; set; }
    public DateTime? ActualEnd { get; set; }
    public string ColorKeyByOrder => OrderNo ?? TaskName;
    public string ColorKeyByPart => PartNo ?? MoldNo ?? TaskName;
    public string ColorKeyByOperation => $"{OperationName}-{OpSeq}";
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
