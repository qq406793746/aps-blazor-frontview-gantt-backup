window.ganttAsprova = window.ganttAsprova || {};

(function () {
    let cleanup = null;
    let observer = null;
    let hoverTaskId = 0;
    let rafPending = false;

    function getHost() {
        return document.querySelector(".asprova-gantt-host")
            || document.querySelector(".syncfusion-gantt-wrap")
            || document.querySelector(".e-gantt");
    }

    function getResourcePane() {
        return document.querySelector(".asprova-resource-pane");
    }

    function getGanttInstance() {
        const gantt = document.querySelector(".e-gantt");
        return gantt && gantt.ej2_instances && gantt.ej2_instances.length > 0
            ? gantt.ej2_instances[0]
            : null;
    }

    function getContent() {
        return document.querySelector(".e-chart-scroll-container")
            || document.querySelector(".e-content")
            || document.querySelector(".e-gantt-content");
    }

    function getChartScroller() {
        const candidates = [
            ".e-chart-scroll-container",
            ".e-gantt-content",
            ".e-content"
        ];
        for (let i = 0; i < candidates.length; i += 1) {
            const el = document.querySelector(candidates[i]);
            if (!el) {
                continue;
            }

            if ((el.scrollWidth - el.clientWidth) > 1 || (el.scrollHeight - el.clientHeight) > 1) {
                return el;
            }
        }

        return getContent();
    }

    function getMetrics() {
        const host = getHost();
        const content = getContent();
        const resourcePane = getResourcePane();
        const timeline = document.querySelector(".e-timeline-header-container");
        const timelineTable = timeline
            ? timeline.querySelector("table")
            : null;
        const chartRow = document.querySelector(".e-chart-row") || document.querySelector(".e-row");
        const chart = document.querySelector(".e-gantt-chart");
        const gantt = document.querySelector(".e-gantt");
        const ganttInstance = gantt && gantt.ej2_instances && gantt.ej2_instances.length > 0
            ? gantt.ej2_instances[0]
            : null;

        const hostRect = host ? host.getBoundingClientRect() : { left: 0, top: 0 };
        const contentRect = content ? content.getBoundingClientRect() : { left: 0, top: 0, width: 0, height: 0 };
        const scrollLeft = content ? content.scrollLeft : 0;
        const scrollTop = content ? content.scrollTop : 0;
        const contentClientWidth = content ? content.clientWidth : 0;
        const contentClientHeight = content ? content.clientHeight : 0;
        const contentScrollWidth = content ? content.scrollWidth : 0;
        const contentScrollHeight = content ? content.scrollHeight : 0;
        const timelineWidth = timeline ? timeline.getBoundingClientRect().width : 0;
        const timelineContentWidth = timelineTable
            ? timelineTable.getBoundingClientRect().width
            : timelineWidth;
        let timelineVisibleWidth = contentClientWidth;
        if (timeline && content) {
            const headerCells = timeline.querySelectorAll("th, .e-timeline-top-header-cell, .e-timeline-header-cell");
            let maxRight = 0;
            headerCells.forEach((cell) => {
                const rect = cell.getBoundingClientRect();
                const rightInContent = rect.right - contentRect.left;
                if (Number.isFinite(rightInContent) && rightInContent > maxRight) {
                    maxRight = rightInContent;
                }
            });
            if (maxRight > 0) {
                timelineVisibleWidth = Math.max(0, Math.min(contentClientWidth, maxRight));
            }
        }
        const chartHeight = chart ? chart.getBoundingClientRect().height : 0;
        const rowHeight = chartRow ? chartRow.getBoundingClientRect().height : 0;
        const pxPerHour = timelineWidth > 0 ? timelineWidth / (24 * 14) : 0;
        const pxPerDay = ganttInstance
            ? (ganttInstance.perDayWidth
                || (ganttInstance.timelineModule ? ganttInstance.timelineModule.perDayWidth : 0)
                || 0)
            : 0;
        const taskbarCount = document.querySelectorAll(".e-taskbar-main-container, .e-gantt-child-taskbar").length;
        const timelineStart = ganttInstance && ganttInstance.timelineModule
            ? (ganttInstance.timelineModule.timelineStartDate || ganttInstance.timelineModule.customTimelineSettings?.timelineStartDate || null)
            : null;
        const timelineEnd = ganttInstance && ganttInstance.timelineModule
            ? (ganttInstance.timelineModule.timelineEndDate || ganttInstance.timelineModule.customTimelineSettings?.timelineEndDate || null)
            : null;
        const resourceRows = [];
        if (resourcePane && host) {
            const rows = resourcePane.querySelectorAll(".as-resource-row[data-resource-id]");
            rows.forEach((row) => {
                const idRaw = row.getAttribute("data-resource-id");
                const resourceId = Number.parseInt(idRaw || "0", 10);
                if (!Number.isFinite(resourceId) || resourceId <= 0) {
                    return;
                }

                const rect = row.getBoundingClientRect();
                resourceRows.push({
                    resourceId,
                    top: rect.top - hostRect.top,
                    height: rect.height
                });
            });
        }

        return {
            scrollLeft,
            scrollTop,
            timelineWidth,
            timelineContentWidth,
            timelineVisibleWidth,
            chartHeight,
            rowHeight,
            pxPerHour,
            pxPerDay,
            contentLeft: Math.max(0, contentRect.left - hostRect.left),
            contentTop: Math.max(0, contentRect.top - hostRect.top),
            contentClientWidth,
            contentClientHeight,
            contentScrollWidth,
            contentScrollHeight,
            taskbarCount,
            timelineStart: timelineStart ? new Date(timelineStart).toISOString() : null,
            timelineEnd: timelineEnd ? new Date(timelineEnd).toISOString() : null,
            resourceRows
        };
    }

    function isTaskbarClick(target) {
        if (!target || !target.closest) {
            return false;
        }

        const bar = target.closest(
            ".e-gantt-child-taskbar, .e-taskbar-main-container, .e-taskbar-left-resizer, .e-taskbar-right-resizer, .e-child-progress-resizer"
        );
        return !!bar;
    }

    function getTaskIdFromTaskbarArgs(args) {
        if (!args) {
            return 0;
        }

        const data = args.data || args.rowData || args.taskData || null;
        if (!data) {
            return 0;
        }

        const raw = data.TaskId
            ?? data.taskId
            ?? data.id
            ?? (data.ganttProperties ? data.ganttProperties.taskId : null)
            ?? 0;
        const parsed = Number.parseInt(String(raw), 10);
        return Number.isFinite(parsed) ? parsed : 0;
    }

    function getTaskIdFromSelectedRow() {
        const ganttInstance = getGanttInstance();
        if (!ganttInstance) {
            return 0;
        }

        let rowIndex = -1;
        if (Number.isInteger(ganttInstance.selectedRowIndex) && ganttInstance.selectedRowIndex >= 0) {
            rowIndex = ganttInstance.selectedRowIndex;
        } else if (ganttInstance.selectionModule && Array.isArray(ganttInstance.selectionModule.selectedRowIndexes) && ganttInstance.selectionModule.selectedRowIndexes.length > 0) {
            rowIndex = ganttInstance.selectionModule.selectedRowIndexes[0];
        }

        if (rowIndex < 0) {
            return 0;
        }

        const current = Array.isArray(ganttInstance.currentViewData)
            ? ganttInstance.currentViewData[rowIndex]
            : null;
        if (!current) {
            return 0;
        }

        const raw = current.TaskId
            ?? current.taskId
            ?? current.ganttProperties?.taskId
            ?? current.taskData?.TaskId
            ?? current.taskData?.taskId
            ?? 0;
        const parsed = Number.parseInt(String(raw), 10);
        return Number.isFinite(parsed) ? parsed : 0;
    }

    function bindSync(dotNetRef) {
        if (!dotNetRef) {
            return;
        }

        const content = getContent();
        const host = getHost();
        const resourcePane = getResourcePane();
        if (!content || !host) {
            return;
        }

        if (cleanup) {
            cleanup();
            cleanup = null;
        }

        const emitViewport = function () {
            if (rafPending) {
                return;
            }

            rafPending = true;
            requestAnimationFrame(function () {
                rafPending = false;
                dotNetRef.invokeMethodAsync("OnGanttViewportChanged", getMetrics()).catch(() => {});
            });
        };

        const onMouseMove = function (ev) {
            const taskId = isTaskbarClick(ev.target) ? 1 : 0;
            if (taskId !== hoverTaskId) {
                hoverTaskId = taskId;
                dotNetRef.invokeMethodAsync("OnGanttTaskHovered", hoverTaskId).catch(() => {});
            }
        };

        const onMouseLeave = function () {
            if (hoverTaskId !== 0) {
                hoverTaskId = 0;
                dotNetRef.invokeMethodAsync("OnGanttTaskHovered", 0).catch(() => {});
            }
        };

        const onResize = function () {
            emitViewport();
        };

        const onHostClick = function (ev) {
            const target = ev.target;
            const taskbarClicked = isTaskbarClick(target);
            if (!taskbarClicked) {
                dotNetRef.invokeMethodAsync("OnGanttTaskbarClicked", 0).catch(() => {});
            } else {
                // Fallback path: when taskbarClick args are unavailable, infer from selected row.
                setTimeout(function () {
                    const selectedTaskId = getTaskIdFromSelectedRow();
                    dotNetRef.invokeMethodAsync("OnGanttTaskbarClicked", selectedTaskId).catch(() => {});
                }, 0);
            }
            const item = target && target.closest
                ? target.closest(".e-toolbar-item, .e-tbar-btn, button")
                : null;
            if (!item) {
                return;
            }

            const raw = ((item.getAttribute && (item.getAttribute("title") || item.getAttribute("aria-label"))) || item.textContent || "")
                .toLowerCase()
                .trim();

            let command = null;
            if (raw.includes("zoom in") || raw.includes("zoomin")) {
                command = "in";
            } else if (raw.includes("zoom out") || raw.includes("zoomout")) {
                command = "out";
            } else if (raw.includes("zoom to fit") || raw.includes("zoomtofit")) {
                command = "fit";
            }

            if (!command) {
                return;
            }

            dotNetRef.invokeMethodAsync("OnGanttZoomCommand", command).catch(() => {});
            emitViewport();
            setTimeout(emitViewport, 80);
            setTimeout(emitViewport, 220);
        };

        let syncingScroll = false;
        const onChartScroll = function () {
            emitViewport();
            if (!resourcePane || syncingScroll) {
                return;
            }

            syncingScroll = true;
            resourcePane.scrollTop = content.scrollTop;
            syncingScroll = false;
        };

        const onResourceScroll = function () {
            if (!resourcePane || syncingScroll) {
                return;
            }

            syncingScroll = true;
            content.scrollTop = resourcePane.scrollTop;
            syncingScroll = false;
            emitViewport();
        };

        let restoreTaskbarClick = null;
        const ganttInstance = getGanttInstance();
        if (ganttInstance) {
            const previousTaskbarClick = ganttInstance.taskbarClick;
            const wrappedTaskbarClick = function (args) {
                let taskId = getTaskIdFromTaskbarArgs(args);
                if (!taskId) {
                    taskId = getTaskIdFromSelectedRow();
                }
                dotNetRef.invokeMethodAsync("OnGanttTaskbarClicked", taskId).catch(() => {});

                if (typeof previousTaskbarClick === "function") {
                    try {
                        previousTaskbarClick.call(this, args);
                    } catch (_) {
                        // no-op
                    }
                }
            };

            ganttInstance.taskbarClick = wrappedTaskbarClick;
            restoreTaskbarClick = function () {
                if (ganttInstance.taskbarClick === wrappedTaskbarClick) {
                    ganttInstance.taskbarClick = previousTaskbarClick;
                }
            };
        }

        content.addEventListener("scroll", onChartScroll, { passive: true });
        if (resourcePane) {
            resourcePane.addEventListener("scroll", onResourceScroll, { passive: true });
        }
        host.addEventListener("mousemove", onMouseMove);
        host.addEventListener("mouseleave", onMouseLeave);
        host.addEventListener("click", onHostClick, true);
        window.addEventListener("resize", onResize);

        const timelineHost = document.querySelector(".e-timeline-header-container") || host;
        if (timelineHost && window.MutationObserver) {
            observer = new MutationObserver(function () {
                emitViewport();
            });
            observer.observe(timelineHost, {
                childList: true,
                subtree: true,
                attributes: true
            });
        }
        emitViewport();

        cleanup = function () {
            content.removeEventListener("scroll", onChartScroll);
            if (resourcePane) {
                resourcePane.removeEventListener("scroll", onResourceScroll);
            }
            host.removeEventListener("mousemove", onMouseMove);
            host.removeEventListener("mouseleave", onMouseLeave);
            host.removeEventListener("click", onHostClick, true);
            window.removeEventListener("resize", onResize);
            if (observer) {
                observer.disconnect();
                observer = null;
            }
            if (restoreTaskbarClick) {
                restoreTaskbarClick();
                restoreTaskbarClick = null;
            }
        };
    }

    window.ganttAsprova.getMetrics = getMetrics;
    window.ganttAsprova.bindSync = bindSync;
    window.ganttAsprova.scrollTimelineToStart = function () {
        const content = getContent();
        if (!content) {
            return;
        }

        content.scrollLeft = 0;
    };
    window.ganttAsprova.restoreScroll = function (left, top) {
        const content = getContent();
        if (!content) {
            return;
        }

        if (Number.isFinite(left)) {
            content.scrollLeft = Math.max(0, left);
        }
        if (Number.isFinite(top)) {
            content.scrollTop = Math.max(0, top);
        }
    };
    window.ganttAsprova.refocusTask = async function (taskId, startAt, endAt) {
        const parsedTaskId = Number.parseInt(String(taskId || "0"), 10);
        if (!Number.isFinite(parsedTaskId) || parsedTaskId <= 0) {
            return false;
        }

        const getTaskRowIndex = function () {
            const gantt = document.querySelector(".e-gantt");
            const ganttInstance = gantt && gantt.ej2_instances && gantt.ej2_instances.length > 0
                ? gantt.ej2_instances[0]
                : null;
            const flatData = ganttInstance && Array.isArray(ganttInstance.flatData)
                ? ganttInstance.flatData
                : null;
            if (!flatData) {
                return -1;
            }

            for (let i = 0; i < flatData.length; i += 1) {
                const row = flatData[i];
                const raw = row?.TaskId
                    ?? row?.taskId
                    ?? row?.ganttProperties?.taskId
                    ?? row?.taskData?.TaskId
                    ?? row?.taskData?.taskId
                    ?? 0;
                const id = Number.parseInt(String(raw), 10);
                if (id === parsedTaskId) {
                    return i;
                }
            }

            return -1;
        };

        const tryFocus = function () {
            const content = getChartScroller();
            if (!content) {
                return false;
            }

            const gantt = document.querySelector(".e-gantt");
            const ganttInstance = gantt && gantt.ej2_instances && gantt.ej2_instances.length > 0
                ? gantt.ej2_instances[0]
                : null;
            const rowIndex = getTaskRowIndex();
            let focusedVertically = false;
            if (rowIndex >= 0) {
                const rows = document.querySelectorAll(".e-chart-row, .e-row");
                const row = rows && rowIndex < rows.length ? rows[rowIndex] : null;
                if (row && Number.isFinite(row.offsetTop)) {
                    try {
                        row.scrollIntoView({ block: "center", inline: "nearest" });
                    } catch (_) {
                        // no-op
                    }
                    const nextTop = Math.max(0, row.offsetTop - (content.clientHeight * 0.5) + (row.offsetHeight * 0.5));
                    content.scrollTop = nextTop;
                    focusedVertically = true;
                } else {
                    const firstRow = rows && rows.length > 0 ? rows[0] : null;
                    const rowHeight = firstRow && firstRow.offsetHeight > 0 ? firstRow.offsetHeight : 36;
                    const estimatedTop = Math.max(0, (rowIndex * rowHeight) - (content.clientHeight * 0.5));
                    content.scrollTop = estimatedTop;
                    focusedVertically = true;
                }
            }

            let focusedHorizontally = false;
            const metrics = getMetrics();
            const startMs = Number.parseInt(String(Date.parse(startAt)), 10);
            const endMsRaw = Number.parseInt(String(Date.parse(endAt)), 10);
            const endMs = Number.isFinite(endMsRaw) ? endMsRaw : startMs;
            const timelineStartMs = Number.parseInt(String(Date.parse(metrics.timelineStart || "")), 10);
            const pxPerDay = metrics.pxPerDay || 0;

            if (Number.isFinite(startMs) && Number.isFinite(endMs) && Number.isFinite(timelineStartMs) && pxPerDay > 0) {
                const midMs = startMs + Math.max(0, endMs - startMs) / 2;
                const dayOffset = (midMs - timelineStartMs) / 86400000;
                const midPx = dayOffset * pxPerDay;
                if (Number.isFinite(midPx)) {
                    const nextLeft = Math.max(0, midPx - (content.clientWidth * 0.5));
                    content.scrollLeft = nextLeft;
                    focusedHorizontally = true;
                }
            }

            if (!focusedHorizontally && ganttInstance && Number.isFinite(startMs) && typeof ganttInstance.scrollToDate === "function") {
                try {
                    ganttInstance.scrollToDate(new Date(startMs));
                    focusedHorizontally = true;
                } catch (_) {
                    // no-op
                }
            }

            return focusedHorizontally || focusedVertically;
        };

        for (let attempt = 1; attempt <= 7; attempt += 1) {
            if (tryFocus()) {
                return true;
            }

            await new Promise((resolve) => setTimeout(resolve, 60 + (attempt * 50)));
        }

        return false;
    };
    window.ganttAsprova.hasHorizontalScroll = function () {
        const content = getContent();
        if (!content) {
            return false;
        }

        return (content.scrollWidth - content.clientWidth) > 1;
    };
    window.ganttAsprova.notifyResize = function () {
        try {
            window.dispatchEvent(new Event("resize"));
        } catch (_) {
            // no-op
        }
    };
    window.ganttAsprova.getViewportHeightForElement = function (element, minHeight, bottomPadding) {
        const min = Number.isFinite(minHeight) ? minHeight : 360;
        const pad = Number.isFinite(bottomPadding) ? bottomPadding : 12;
        if (!element || !element.getBoundingClientRect) {
            return min;
        }

        const rect = element.getBoundingClientRect();
        const viewportHeight = window.innerHeight || document.documentElement.clientHeight || 0;
        if (viewportHeight <= 0) {
            return min;
        }

        const available = Math.floor(viewportHeight - rect.top - pad);
        return available > min ? available : min;
    };
    window.ganttAsprova.expandAllRowsSafe = async function () {
        for (let round = 0; round < 4; round += 1) {
            const expands = Array.from(document.querySelectorAll(".e-treegridexpand"));
            if (expands.length === 0) {
                break;
            }

            expands.forEach((el) => {
                try {
                    el.dispatchEvent(new MouseEvent("click", { bubbles: true }));
                } catch (_) {
                    // no-op
                }
            });

            await new Promise((resolve) => setTimeout(resolve, 40));
        }
    };
    window.ganttAsprova.unbindSync = function () {
        if (cleanup) {
            cleanup();
            cleanup = null;
        }
        if (observer) {
            observer.disconnect();
            observer = null;
        }
        hoverTaskId = 0;
        rafPending = false;
    };
})();
