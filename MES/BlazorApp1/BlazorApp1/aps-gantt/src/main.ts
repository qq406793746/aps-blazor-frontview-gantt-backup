import { Gantt, TYPES } from "@visactor/vtable-gantt";
import "./style.css";

type ResourceTask = {
  id: string;
  code?: string;
  name?: string;
  startTime: string;
  endTime: string;
  color?: string;
  urgent?: boolean;
  locked?: boolean;
  meta?: Record<string, string>;
};

type ResourceRecord = {
  id: string;
  name: string;
  code?: string;
  children?: ResourceTask[];
  startTime?: string;
  endTime?: string;
};

type GanttPayload = {
  resources: ResourceRecord[];
  dateRange?: [string, string];
  markLine?: { date: string; color?: string }[];
  links?: { linkedFromTaskKey: string; linkedToTaskKey: string; type?: string }[];
};

type InstanceEntry = {
  gantt: any;
  container: HTMLElement;
  tooltip: HTMLDivElement;
  resizeObserver?: ResizeObserver;
  mutationObserver?: MutationObserver;
};

const instances = new Map<string, InstanceEntry>();
let dotNetRef: any = null;

function createTooltip(): HTMLDivElement {
  const el = document.createElement("div");
  el.style.position = "fixed";
  el.style.zIndex = "10000";
  el.style.minWidth = "220px";
  el.style.background = "#fff";
  el.style.border = "1px solid #e5e7eb";
  el.style.borderRadius = "6px";
  el.style.boxShadow = "0 8px 24px rgba(0,0,0,0.12)";
  el.style.padding = "10px 12px";
  el.style.fontSize = "12px";
  el.style.color = "#111827";
  el.style.display = "none";
  document.body.appendChild(el);
  return el;
}

function renderTooltip(el: HTMLDivElement, task: ResourceTask, x: number, y: number) {
  const lines: string[] = [];
  if (task.code) lines.push(`<div><strong>任务：</strong>${task.code}</div>`);
  if (task.name) lines.push(`<div><strong>工序：</strong>${task.name}</div>`);
  lines.push(`<div><strong>开始：</strong>${task.startTime}</div>`);
  lines.push(`<div><strong>结束：</strong>${task.endTime}</div>`);
  if (task.meta) {
    Object.entries(task.meta).forEach(([k, v]) => {
      lines.push(`<div><strong>${k}：</strong>${v}</div>`);
    });
  }
  el.innerHTML = lines.join("");
  el.style.left = `${x + 12}px`;
  el.style.top = `${y + 12}px`;
  el.style.display = "block";
}

function hideTooltip(el: HTMLDivElement) {
  el.style.display = "none";
}

function computeDateRange(records: ResourceRecord[]): [string, string] {
  let min: Date | null = null;
  let max: Date | null = null;
  records.forEach((r) => {
    (r.children || []).forEach((t) => {
      const s = new Date(t.startTime);
      const e = new Date(t.endTime);
      if (!isNaN(s.getTime()) && (!min || s < min)) min = s;
      if (!isNaN(e.getTime()) && (!max || e > max)) max = e;
    });
  });
  if (!min || !max) {
    const now = new Date();
    const start = new Date(now.getTime() - 24 * 60 * 60 * 1000);
    const end = new Date(now.getTime() + 24 * 60 * 60 * 1000);
    min = start;
    max = end;
  }
  const fmt = (d: Date) =>
    `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}T` +
    `${String(d.getHours()).padStart(2, "0")}:${String(d.getMinutes()).padStart(2, "0")}:${String(d.getSeconds()).padStart(2, "0")}`;
  return [fmt(min), fmt(max)];
}

function buildOption(records: ResourceRecord[], payload: GanttPayload) {
  const option: any = {
    overscrollBehavior: "none",
    records,
    groupBy: true,
    tasksShowMode: TYPES.TasksShowMode.Sub_Tasks_Compact,
    taskListTable: {
      columns: [
        {
          field: "name",
          title: "资源名称",
          width: 260,
          style: { textOverflow: "ellipsis" }
        },
        {
          field: "code",
          title: "编码",
          width: 90,
          style: { textOverflow: "ellipsis" }
        }
      ],
      tableWidth: 360,
      minTableWidth: 280,
      maxTableWidth: 600
    },
    grid: {
      verticalLine: { lineWidth: 1, lineColor: "#EEEFF0" },
      horizontalLine: { lineWidth: 1, lineColor: "#EEEFF0" }
    },
    frame: {
      outerFrameStyle: {
        borderLineWidth: 1,
        borderColor: "#EEEFF0",
        cornerRadius: 0
      },
      verticalSplitLine: {
        lineColor: "#EEEFF0",
        lineWidth: 4
      },
      horizontalSplitLine: {
        lineColor: "#EEEFF0",
        lineWidth: 1
      },
      verticalSplitLineMoveable: true,
      verticalSplitLineHighlight: {
        lineColor: "#EEEFF0",
        lineWidth: 1
      }
    },
    headerRowHeight: 30,
    rowHeight: 46,
    taskBar: {
      startDateField: "startTime",
      endDateField: "endTime",
      progressField: "progress",
      moveable: true,
      resizable: false,
      barStyle: (args: any) => {
        const task: ResourceTask = args?.record || {};
        const color = task.color || (task.urgent ? "#ef4444" : "#60a5fa");
        return {
          width: 36,
          cornerRadius: 4,
          fill: color,
          stroke: task.locked ? "#0f172a" : color,
          lineWidth: task.locked ? 2 : 1
        };
      }
    }
  };

  if (payload.links && payload.links.length > 0) {
    option.dependency = {
      links: payload.links,
      linkLineStyle: { lineColor: "#94a3b8", lineWidth: 1 },
      linkSelectedLineStyle: { lineColor: "#64748b", lineWidth: 2 },
      linkCreatable: false,
      linkSelectable: false,
      linkDeletable: false,
      distanceToTaskBar: 8
    };
  }

  const range =
    payload.dateRange && payload.dateRange.length === 2
      ? payload.dateRange
      : computeDateRange(records);
  option.minDate = range[0];
  option.maxDate = range[1];

  if (payload.markLine && payload.markLine.length > 0) {
    option.markLine = payload.markLine.map((m) => ({
      date: new Date(m.date),
      position: "middle",
      style: {
        lineColor: m.color || "red",
        lineWidth: 1,
        lineDash: [4, 4]
      }
    }));
  }

  return option;
}

function attachEvents(entry: InstanceEntry) {
  const { gantt, tooltip } = entry;

  gantt.on("mouseenter_task_bar", (data: any) => {
    const task = data?.record as ResourceTask | undefined;
    if (!task) return;
    renderTooltip(tooltip, task, data?.event?.clientX ?? 0, data?.event?.clientY ?? 0);
  });

  gantt.on("mouseleave_task_bar", () => hideTooltip(tooltip));

  gantt.on("move_end_task_bar", (data: any) => {
    const task = data?.record as ResourceTask | undefined;
    if (!task) return;
    const detail = {
      id: task.id,
      startTime: data?.record?.startTime,
      endTime: data?.record?.endTime
    };
    window.dispatchEvent(new CustomEvent("aps-resource-gantt:drag-end", { detail }));
    if (dotNetRef?.invokeMethodAsync) {
      dotNetRef.invokeMethodAsync("OnResourceGanttDragEnd", detail);
    }
  });
}

function normalizeResources(payload: GanttPayload): ResourceRecord[] {
  return (payload.resources || []).map((r) => {
    if (!r.children || r.children.length === 0) return r;
    let min = r.startTime;
    let max = r.endTime;
    r.children.forEach((t) => {
      if (!min || new Date(t.startTime) < new Date(min)) min = t.startTime;
      if (!max || new Date(t.endTime) > new Date(max)) max = t.endTime;
    });
    return { ...r, startTime: min, endTime: max };
  });
}

function lockContainerWidth(container: HTMLElement): number {
  const parent = container.parentElement;
  const baseWidth = parent?.clientWidth || container.clientWidth || 1200;
  container.style.width = `${baseWidth}px`;
  container.style.minWidth = `${baseWidth}px`;
  container.style.maxWidth = `${baseWidth}px`;
  container.style.overflowX = "hidden";
  container.style.boxSizing = "border-box";
  container.style.display = "block";
  container.style.contain = "layout paint size";
  return baseWidth;
}

function clampLayout(container: HTMLElement) {
  const parent = container.parentElement;
  const baseWidth = parent?.clientWidth || container.clientWidth || 1200;
  container.style.width = `${baseWidth}px`;
  container.style.minWidth = `${baseWidth}px`;
  container.style.maxWidth = `${baseWidth}px`;
  container.style.overflowX = "hidden";
  const candidates = container.querySelectorAll<HTMLElement>("canvas, svg, div");
  candidates.forEach((el) => {
    const sw = el.scrollWidth || 0;
    if (sw > baseWidth * 1.05) {
      el.style.width = `${baseWidth}px`;
      el.style.maxWidth = `${baseWidth}px`;
      el.style.overflow = "hidden";
    }
    if (el instanceof HTMLCanvasElement) {
      el.style.width = `${baseWidth}px`;
    }
    const widthAttr = el.getAttribute("width");
    if (widthAttr) {
      const w = Number(widthAttr);
      if (Number.isFinite(w) && w > baseWidth) {
        el.setAttribute("width", `${baseWidth}`);
      }
    }
  });
}

function render(containerId: string, payload: GanttPayload) {
  const container = document.getElementById(containerId);
  if (!container) return;

  const shell = container.parentElement;
  if (shell) {
    shell.style.width = "100%";
    shell.style.maxWidth = "100%";
    shell.style.height = shell.style.height || "600px";
    shell.style.overflow = "hidden";
    shell.style.position = "relative";
  }
  container.style.width = "100%";
  container.style.height = "100%";
  container.style.position = "absolute";
  container.style.inset = "0";
  lockContainerWidth(container);

  const records = normalizeResources(payload);
  if (records.length === 0) {
    const existing = instances.get(containerId);
    if (existing) {
      existing.gantt.setRecords([]);
    }
    return;
  }
  const option = buildOption(records, payload);

  const existing = instances.get(containerId);
  if (existing) {
    existing.gantt.setRecords(records);
    if (payload.dateRange && payload.dateRange.length === 2) {
      existing.gantt.updateDateRange(payload.dateRange[0], payload.dateRange[1]);
    }
    lockContainerWidth(existing.container);
    clampLayout(existing.container);
    return;
  }

  const tooltip = createTooltip();
  const gantt = new Gantt(container, option);
  const entry: InstanceEntry = { gantt, container, tooltip };
  attachEvents(entry);
  if (parent && "ResizeObserver" in window) {
    const ro = new ResizeObserver(() => {
      lockContainerWidth(container);
      clampLayout(container);
    });
    ro.observe(parent);
    entry.resizeObserver = ro;
  }
  if ("MutationObserver" in window) {
    const mo = new MutationObserver(() => {
      clampLayout(container);
    });
    mo.observe(container, { childList: true, subtree: true, attributes: true });
    entry.mutationObserver = mo;
  }
  clampLayout(container);
  instances.set(containerId, entry);
}

function destroy(containerId: string) {
  const entry = instances.get(containerId);
  if (!entry) return;
  entry.tooltip.remove();
  entry.resizeObserver?.disconnect();
  entry.mutationObserver?.disconnect();
  entry.gantt?.release?.();
  instances.delete(containerId);
}

function registerDotNet(ref: any) {
  dotNetRef = ref;
}

(window as any).ApsResourceGantt = { render, destroy, registerDotNet };
