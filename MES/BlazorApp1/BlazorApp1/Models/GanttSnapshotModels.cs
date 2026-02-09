using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorApp1.Models;

public class ResourceGanttSnapshot
{
    public GanttMeta Meta { get; set; } = new();
    public IReadOnlyList<GanttResource> Resources { get; set; } = Array.Empty<GanttResource>();
    public IReadOnlyList<GanttCalendar> Calendars { get; set; } = Array.Empty<GanttCalendar>();
    public IReadOnlyList<GanttAssignment> Assignments { get; set; } = Array.Empty<GanttAssignment>();
    public IReadOnlyList<GanttLink> Links { get; set; } = Array.Empty<GanttLink>();
}

public class GanttMeta
{
    public long PlanId { get; set; }
    public string PlanNo { get; set; } = "";
    public DateTime PlanDueDate { get; set; }
    public DateTime? PlanningStartTime { get; set; }
    public DateTime? FreezeEndTime { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public string LinksMode { get; set; } = "seq";
    public string? Timezone { get; set; }

    [JsonPropertyName("windowStart")]
    public DateTime WindowStart
    {
        get => From;
        set => From = value;
    }

    [JsonPropertyName("windowEnd")]
    public DateTime WindowEnd
    {
        get => To;
        set => To = value;
    }
}

public class GanttResource
{
    public long Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "Machine";
    public string Status { get; set; } = "";
    public int Capacity { get; set; } = 1;
    public long CalendarId { get; set; }
}

public class GanttCalendar
{
    public long Id { get; set; }
    public string ResourceType { get; set; } = "Machine";
    public long ResourceId { get; set; }
    public string Name { get; set; } = "";
    public bool IsActive { get; set; }
    public IReadOnlyList<GanttInterval> WorkingIntervals { get; set; } = Array.Empty<GanttInterval>();
    public IReadOnlyList<GanttDowntime> Downtimes { get; set; } = Array.Empty<GanttDowntime>();
}

public class GanttInterval
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public class GanttDowntime
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Reason { get; set; }
}

public class GanttAssignment
{
    private long _resourceId;
    private string _resourceType = "Machine";

    public long TaskId { get; set; }
    public long PlanId { get; set; }
    public string ResourceType
    {
        get => _resourceType;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _resourceType = value;
            }
            else if (string.IsNullOrWhiteSpace(_resourceType))
            {
                _resourceType = "Machine";
            }
        }
    }
    public long ResourceId
    {
        get => _resourceId;
        set
        {
            // Keep alias-derived resource id when payload later writes 0.
            if (value > 0 || _resourceId == 0)
            {
                _resourceId = value;
            }
        }
    }
    public long? WorkItemId { get; set; }
    public string? OrderNo { get; set; }
    public string? ItemHint { get; set; }
    public long OperationId { get; set; }
    public string OperationName { get; set; } = "";
    public int OpSeq { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public DateTime? EarliestStart { get; set; }
    public DateTime? LatestEnd { get; set; }
    public DateTime? DueDate { get; set; }
    public int Priority { get; set; }
    public decimal Qty { get; set; }
    public string Status { get; set; } = "";
    public DateTime? ActualStart { get; set; }
    public DateTime? ActualEnd { get; set; }
    public bool IsLocked { get; set; }
    public bool IsUrgent { get; set; }
    public int UrgentLevel { get; set; }
    public bool IsOutsourced { get; set; }
    public long? VendorId { get; set; }
    public int SetupMinutes { get; set; }
    public bool IsLate { get; set; }
    public IReadOnlyList<GanttViolation> Violations { get; set; } = Array.Empty<GanttViolation>();
    public GanttDisplay Display { get; set; } = new();

    [JsonPropertyName("plannedStart")]
    public DateTime PlannedStart
    {
        get => Start;
        set => Start = value;
    }

    [JsonPropertyName("plannedEnd")]
    public DateTime PlannedEnd
    {
        get => End;
        set => End = value;
    }

    [JsonPropertyName("startTime")]
    public DateTime StartTime
    {
        get => Start;
        set => Start = value;
    }

    [JsonPropertyName("endTime")]
    public DateTime EndTime
    {
        get => End;
        set => End = value;
    }

    [JsonPropertyName("machineId")]
    public long? MachineId
    {
        get => null;
        set
        {
            if (value.HasValue && value.Value > 0)
            {
                ResourceId = value.Value;
            }
            ResourceType = "Machine";
        }
    }

    [JsonPropertyName("assignedPersonId")]
    public long? AssignedPersonId
    {
        get => null;
        set
        {
            if (value.HasValue && value.Value > 0)
            {
                ResourceId = value.Value;
            }
            ResourceType = "Person";
        }
    }

    [JsonPropertyName("personId")]
    public long? PersonId
    {
        get => null;
        set
        {
            if (value.HasValue && value.Value > 0)
            {
                ResourceId = value.Value;
            }
            ResourceType = "Person";
        }
    }

    [JsonPropertyName("toolId")]
    public long? ToolId
    {
        get => null;
        set
        {
            if (value.HasValue && value.Value > 0)
            {
                ResourceId = value.Value;
            }
            ResourceType = "Tool";
        }
    }

    [JsonPropertyName("vendorResourceId")]
    public long? VendorResourceId
    {
        get => null;
        set
        {
            if (value.HasValue && value.Value > 0)
            {
                ResourceId = value.Value;
            }
            ResourceType = "Vendor";
        }
    }
}

public class GanttViolation
{
    public string Code { get; set; } = "";
    public string Message { get; set; } = "";
}

public class GanttDisplay
{
    public IReadOnlyList<string> LabelLines { get; set; } = Array.Empty<string>();
    public string ColorKey { get; set; } = "workItem";
}

public class GanttLink
{
    public long FromTaskId { get; set; }
    public long ToTaskId { get; set; }
    public string Type { get; set; } = "FS";
    public string DerivedFrom { get; set; } = "seq";
    public long WorkItemId { get; set; }
}

public class GanttLinkPath
{
    public string Path { get; set; } = "";
    public bool IsSelected { get; set; }
}

public class GanttBarHoverEvent
{
    public GanttAssignment Assignment { get; set; } = new();
    public double ClientX { get; set; }
    public double ClientY { get; set; }
}
