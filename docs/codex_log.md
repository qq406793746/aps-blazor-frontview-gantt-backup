## 2026-01-26 17:00

- Goal: complete first-pass integration between Dynamic Scheduling page and gantt scripts.
- Update time: 2026-01-26 17:00
- Files changed:
  - MES/BlazorApp1/BlazorApp1/Pages/DynamicScheduling.razor
  - MES/BlazorApp1/BlazorApp1/wwwroot/js/aps-gantt.js
  - MES/BlazorApp1/BlazorApp1/wwwroot/aps-gantt/aps-resource-gantt.js
- Verification:
  - Dynamic Scheduling page opens in BlazorApp1.
  - Gantt scripts load and basic rendering works.
  - Reload/re-enter does not throw front-end errors.
- Open items:
  - Project dropdown init was still unstable at this point (fixed at 2026-01-26 20:10).
  - Empty-state and error prompt handling still needed.


## 2026-01-26 20:10

- 症状：动态排产页面可打开，但“项目”下拉为空；直接访问 WebApi `/api/projects` 有数据返回。
- 原因：项目列表加载放在 `OnAfterRenderAsync`，前序 JS 互操作在首帧失败/短路时导致未执行 `RefreshLists`，因此下拉未初始化。
- 处理：将列表初始化从 `OnAfterRenderAsync` 移到 `OnInitializedAsync`，使数据加载与 JS 初始化解耦，页面一加载即拉项目。
- 修改时间：2026-01-26 20:10
- 改了哪些文件：
  - MES\BlazorApp1\BlazorApp1\Pages\DynamicScheduling.razor

## 2026-01-28 资源甘特图（Asprova 风格）

- 目标：在动态排产下新增资源甘特图页面 /gantt/resource，支持资源行、可作业灰底、作业块、逾期红字、违约蓝字、连线、高亮链路、缩放与过滤。
- 修改时间：2026-01-28
- 改了哪些文件：
  - MES\BlazorApp1\BlazorApp1\Models\GanttSnapshotModels.cs
  - MES\BlazorApp1\BlazorApp1\Services\GanttApiClient.cs
  - MES\BlazorApp1\BlazorApp1\Components\Gantt\ResourceGantt.razor
  - MES\BlazorApp1\BlazorApp1\Components\Gantt\ResourceList.razor
  - MES\BlazorApp1\BlazorApp1\Components\Gantt\TimelineHeader.razor
  - MES\BlazorApp1\BlazorApp1\Components\Gantt\GanttCanvas.razor
  - MES\BlazorApp1\BlazorApp1\Pages\GanttResource.razor
  - MES\BlazorApp1\BlazorApp1\wwwroot\js\resource-gantt.js
  - MES\BlazorApp1\BlazorApp1\wwwroot\css\site.css
  - MES\BlazorApp1\BlazorApp1\Pages\_Layout.cshtml
  - MES\BlazorApp1\BlazorApp1\Program.cs
  - MES\BlazorApp1\BlazorApp1\_Imports.razor
  - MES\BlazorApp1\BlazorApp1\Pages\DynamicScheduling.razor
- 如何验证：
  - 确认 appsettings 中 ApsApi.BaseUrl 指向后端（如 http://localhost:5024/）
  - 启动 BlazorApp1，访问 /gantt/resource
  - 选择 PlanId 后点击“刷新”，确认资源行、灰底、作业块、连线与高亮正常
  - 缩放/过滤/高亮链路/点击任务显示详情无异常
- 未解决事项：
  - 左侧导航未新增入口（目前通过动态排产页按钮或直接访问 /gantt/resource）
  - 未实现导出 PNG/SVG 与多项目下拉筛选
## 2026-01-28 资源甘特图修复（作业条不显示）

- 目标：修复资源甘特图作业条不显示、全落入未分配/过滤为空的问题。
- 修改时间：2026-01-28 15:13
- 改了哪些文件：
  - MES\BlazorApp1\BlazorApp1\Components\Gantt\ResourceGantt.razor
  - MES\BlazorApp1\BlazorApp1\Components\Gantt\GanttCanvas.razor
  - MES\BlazorApp1\BlazorApp1\Models\GanttSnapshotModels.cs
- 如何验证：
  - 启动 BlazorApp1，访问 /gantt/resource
  - 输入有效 PlanId，点击“刷新”
  - 顶部 debug 行确认 SnapshotAssignments>0、FilteredAssignments>0、Buckets>0
  - 画布中可看到作业条（可能在 Unassigned 行或对应资源行）
- 未解决事项：
  - 仍需确认后端返回字段是否与前端 DTO 完全一致（如 resourceType/resourceId 字段命名差异）。

## 2026-01-29 Resource Gantt debug + sidebar toggle

- Goal: add a collapsible left sidebar toggle, improve resource gantt readability, and add debug overlay/console diagnostics.
- Changes:
  - Sidebar toggle button in layout (top of left nav) and collapse behavior.
  - Resource list labels cleaned with fallbacks for garbled names; ellipsis for overflow.
  - Task bars compact mode hides labels when too narrow; debug overlay shows taskId/x/width/rowIndex when debug=1.
  - Debug logging on bar click outputs DOMRect, scrollLeft, pxPerMinute.
- Files:
  - MES\BlazorApp1\BlazorApp1\Shared\MainLayout.razor
  - MES\BlazorApp1\BlazorApp1\Shared\MainLayout.razor.css
  - MES\BlazorApp1\BlazorApp1\Components\Gantt\GanttCanvas.razor
  - MES\BlazorApp1\BlazorApp1\Components\Gantt\ResourceGantt.razor
  - MES\BlazorApp1\BlazorApp1\Components\Gantt\ResourceList.razor
  - MES\BlazorApp1\BlazorApp1\Pages\GanttResource.razor
  - MES\BlazorApp1\BlazorApp1\wwwroot\js\resource-gantt.js
  - MES\BlazorApp1\BlazorApp1\wwwroot\css\site.css
  - docs\codex_log.md
- How to verify:
  - Visit /gantt/resource?debug=1, click a bar; check console logs.
  - Confirm debug overlay text appears on bars when debug=1.
  - Toggle sidebar visibility via the button.

## 2026-01-29 Resource Gantt UX fixes

- Goal: align resource list and gantt rows, improve label readability, hide right scrollbar, and stabilize sidebar collapse behavior.
- Changes:
  - Resource list rows bound to RowHeight; gantt rows set explicit RowHeight to prevent vertical drift.
  - Right canvas scrollbar hidden; left scrollbar kept; scroll sync padding added to align ranges.
  - Resource labels now use Chinese “编码/类型” with type mapping; garbled names filtered out.
  - Task bar labels prioritize part (ItemHint), then operation, then order; adaptive line count by bar width.
  - Sidebar collapse hides content container instead of only frame.
- Files:
  - MES\BlazorApp1\BlazorApp1\Components\Gantt\GanttCanvas.razor
  - MES\BlazorApp1\BlazorApp1\Components\Gantt\ResourceList.razor
  - MES\BlazorApp1\BlazorApp1\Shared\MainLayout.razor
  - MES\BlazorApp1\BlazorApp1\Shared\MainLayout.razor.css
  - MES\BlazorApp1\BlazorApp1\wwwroot\css\site.css
  - MES\BlazorApp1\BlazorApp1\wwwroot\js\resource-gantt.js
  - docs\codex_log.md
- How to verify:
  - Visit /gantt/resource and confirm left/right rows align at scroll bottom.
  - Check resource list labels show “编码/类型” and no “???” names.
  - Confirm right scrollbar hidden while left scrollbar remains.
## 2026-01-29 - Resource Gantt 资源栏宽度可拖拽
- 目标：资源栏内容被截断时可由用户拖拽调整显示宽度。
- 改动：
  - MES/BlazorApp1/BlazorApp1/Components/Gantt/ResourceGantt.razor
    - 为资源栏区域新增拖拽把手，初始化 JS resizer。
  - MES/BlazorApp1/BlazorApp1/wwwroot/css/site.css
    - 引入 CSS 变量 --rg-resource-width 控制资源栏宽度；拖拽条样式与 hover 视觉。
  - MES/BlazorApp1/BlazorApp1/wwwroot/js/resource-gantt.js
    - 新增 initResourceResizer：按住拖拽调整资源栏宽度，范围 180–520px，localStorage 记忆。
- 验证：未运行（需打开资源甘特图手动拖拽验证）。

## 2026-02-02 资源甘特图页面改为 classic APS 风格

- 目标：将“动态排产 -> 打开资源甘特图（新）”对应页面 `/gantt/resource` 的前端布局与视觉，改为 `classic-aps-resource-gantt` 仓库风格（经典 APS 工业界面）。
- 改动：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttResource.razor`
    - 页面整体改为 classic 结构：顶部菜单栏、工具栏、标题栏。
    - 保留原有数据控制能力（PlanId/时间窗/链接模式/过滤/缩放），并调整文案与布局为经典风格。
    - 甘特图外层改为经典面板容器。
    - 新增底部 tabs（订单甘特图/资源甘特图/BOM/订单表）和属性区域（点击任务显示详情）。
    - 新增状态栏（Plan/Resources/Tasks + NUM/CAPS/SCROLL）。
  - `MES/BlazorApp1/BlazorApp1/wwwroot/css/site.css`
    - 新增 `classic-*` 样式块并覆盖 `rg-*` 在该页的视觉表现：
      - 菜单栏/工具栏/标题栏样式；
      - 甘特主区域边框、时间轴、资源列、任务条、连线、当前线；
      - 底部 tab、属性面板、状态栏；
      - 小屏响应式宽度适配。
- 说明：
  - 本次仅改 UI 结构与样式，未改后端 API / 数据模型 / 资源甘特图核心交互逻辑。
- 构建验证：
  - 执行 `dotnet build BlazorApp1.sln` 失败，原因是 `BlazorApp1.exe` 被运行中进程锁定（PID 38944），属于进程占用问题，不是本次代码编译错误。


## 2026-02-03 Resource Gantt classic style + timeline + assignment fixes

- Goal: improve `/gantt/resource` classic UI and fix timeline, assignment mapping, and visibility issues.
- Key changes:
  1. Unified classic visual details and container boundary styles (`site.css` + `ResourceGantt.razor`).
  2. Timeline switched to dual layer (date + hour) for readability (`TimelineHeader.razor`).
  3. Tuned timeline labels/grid/spacing to avoid overlap and jumping (`TimelineHeader.razor` + `site.css`).
  4. Fixed `position/top/z-index` layering so bars are not covered by dim/mask overlays (`GanttCanvas.razor` + `site.css`).
  5. Added assignment fallback: empty `resourceType + ResourceId` goes to `UNASSIGNED` (`GanttCanvas.razor`, `ResourceGantt.razor`).
  6. Relaxed Person/Tool/Outsource assignment filtering to reduce false `Unassigned` cases (`GanttResource.razor`).
  7. Improved `Unassigned` bucketing/sorting to reduce duplicates and row drift (`ResourceGantt.razor`).
  8. Clamped assignment `Start/End` at window bounds to avoid losing bars outside range (`GanttResource.razor`).
  9. Completed classic style mapping for hour/day/shift/frozen zones (`ResourceGantt.razor` + `site.css`).
  10. Corrected `adjustedLeft` calculation to remove horizontal offset (`GanttCanvas.razor`).
- Result:
  - Timeline display is more stable.
  - Bar positioning and masking issues improved.
  - `Unassigned` grouping is more predictable, with better classic UI consistency.
- Files changed:
  - `MES/BlazorApp1/BlazorApp1/Components/Gantt/GanttCanvas.razor`
  - `MES/BlazorApp1/BlazorApp1/Components/Gantt/ResourceGantt.razor`
  - `MES/BlazorApp1/BlazorApp1/Components/Gantt/TimelineHeader.razor`
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttResource.razor`
  - `MES/BlazorApp1/BlazorApp1/wwwroot/css/site.css`
  - `docs/codex_log.md`
- Build validation:
  - `dotnet build BlazorApp1.sln` was still blocked by running process lock on `BlazorApp1.exe` (not a compile error).


## 2026-02-04 - Blazor Syncfusion Gantt Resource View page (/gantt/syncfusion)

- Goal: integrate Syncfusion Blazor Gantt Resource View into existing BlazorApp1 and add a new APS resource gantt page.
- Changes:
  - Added Syncfusion packages: `Syncfusion.Blazor.Gantt`, `Syncfusion.Blazor.Themes` (without installing `Syncfusion.Blazor` meta package).
  - Registered Syncfusion in startup:
    - `builder.Services.AddSyncfusionBlazor()`
    - read `Syncfusion:LicenseKey` from config and call `SyncfusionLicenseProvider.RegisterLicense(...)` when non-empty.
  - Added static assets:
    - Theme CSS: `_content/Syncfusion.Blazor.Themes/bootstrap5.css`
    - Script: `_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js`
    - Page styles: `wwwroot/css/gantt-syncfusion.css`
  - Added new route page `/gantt/syncfusion` with:
    - Resource View rendering (`<SfGantt ViewType="ViewType.ResourceView">`)
    - toolbar zoom actions (ZoomIn/ZoomOut/ZoomToFit)
    - resource/order keyword filtering
    - `Highlight Selected Chain Only` toggle
    - debug overlay with selected task JSON + predecessor/successor BFS chain dump + clipboard copy.
  - Added Syncfusion response view models and extended API client to call:
    - `GET /api/gantt/syncfusion/resource-view?planId=...&start=...&end=...`
  - Added resource assignment mapping via `GanttAssignmentFields` so task/resource relation works in Resource View.
- Validation:
  - `dotnet build MES/BlazorApp1/BlazorApp1/BlazorApp1.csproj` passed (warnings only, no errors).
  - Runtime smoke check:
    - `curl http://localhost:5157/gantt/syncfusion` returned `HTTP=200`
    - HTML contains `syncfusion-gantt-page` and gantt markup.

## 2026-02-04 - Syncfusion gantt follow-up fixes and troubleshooting

- Added/adjusted:
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
    - Added fallback reload when selected `start/end` has no tasks.
    - Added resource backfill from `/api/gantt/resource-snapshot` assignments when `resource-view` tasks have empty `resourceIds`.
    - Added task-resource fallback (`Unassigned`) when still empty.
    - Added chart range auto calculation (`ProjectStartDate/ProjectEndDate`) from task min/max time.
    - Added debug switch `Debug ProjectView` and rendered ProjectView/ResourceView as separate branches to avoid runtime interop disconnect caused by dynamic view switching.
    - Added summary counters (`TaskResMapped`, `Assignments`) for runtime diagnosis.
  - `MES/BlazorApp1/BlazorApp1/Models/SyncfusionGanttModels.cs`
    - Added safe duration deserialization fields and expansion flag mapping.
  - `MES/BlazorApp1/BlazorApp1/wwwroot/css/gantt-syncfusion.css`
    - Added forced high-contrast taskbar style for visibility troubleshooting.
  - `MES/BlazorApp1/BlazorApp1/appsettings.json`
  - `MES/BlazorApp1/BlazorApp1/appsettings.Development.json`
    - Cleared hard-coded Syncfusion key; switched to environment variable usage (`Syncfusion__LicenseKey`).
- Operational:
  - Set environment variable using PowerShell: `setx Syncfusion__LicenseKey "..."`
- Observed status:
  - ProjectView can render task bars.
  - ResourceView currently still aggregates to one resource row with no expected multi-resource bar layout; further ResourceView mapping alignment is still needed.

## 2026-02-04 - Asprova-style milestone1 enhancement (Syncfusion Resource View)

- Updated `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`:
  - Switched frontend data loading to one aggregate API call (`/api/gantt/resource-snapshot`) and removed resource-view + backfill split flow.
  - Added Asprova-like left resource pane (grouped, collapsible, two-line info, type color strip).
  - Added `ColorMode` (ByOrder/ByPart/ByOperation/ByResourceGroup) and deterministic color bucketing.
  - Added `LinkMode` (All/HoverOnly/SelectedPath) + selected-chain BFS highlighting and non-chain dimming.
  - Added row click selection (`RowSelected`) and bottom property panel (resource/time/order/op/flags/anomaly).
  - Added DebugOverlay enhancements: Copy JSON + Dump visible rect + chain/viewport metrics.
  - Added flags computation (`overdue`, `precedenceViolation`, `resourceConflict`) and red/blue text class mapping.
- Updated `MES/BlazorApp1/BlazorApp1/Models/SyncfusionGanttModels.cs`:
  - Cleaned JSON attributes causing property-name collision risk (`duration`) and aligned VM fields for local rendering use.
- Added `MES/BlazorApp1/BlazorApp1/wwwroot/css/gantt-asprova.css`:
  - Centralized Asprova-like styles (resource pane, type strips, task color buckets, chain highlight/mute, bottom panel).
- Added `MES/BlazorApp1/BlazorApp1/wwwroot/js/gantt-asprova.js`:
  - Added lightweight viewport metric helper for DebugOverlay (`scrollLeft`, `scrollTop`, `rowHeight`, etc.).
- Updated `MES/BlazorApp1/BlazorApp1/Pages/_Layout.cshtml`:
  - Included `css/gantt-asprova.css` and `js/gantt-asprova.js`.
- Validation:
  - `dotnet build ... -p:UseAppHost=false` reached compile phase without new C#/Razor errors for these changes.
  - Build copy step failed due existing running process lock on `bin\\Debug\\net8.0\\BlazorApp1.dll` (runtime process remained active).

## 2026-02-04 - Hotfix: duplicate resource key crash in Syncfusion Gantt

- Updated `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`:
  - Fixed `System.ArgumentException: An item with the same key has already been added` in `GanttResource.GenerateResources`.
  - Root cause: backend can return same numeric `ResourceId` across different `ResourceType`, which collided in Syncfusion resource dictionary.
  - Added frontend resource-id remapping (`type + rawId -> unique int`) and used the remapped id consistently for:
    - `DisplayResources` (`GanttResource.Id`)
    - task `ResourceIds`
    - assignment `ResourceId`
  - Added resource deduping by remapped id as extra guard.
- Validation:
  - `dotnet build MES/BlazorApp1/BlazorApp1/BlazorApp1.csproj -p:UseAppHost=false -p:OutDir=... -p:IntermediateOutputPath=...` passed (`0 errors`).

## 2026-02-04 - Hotfix: bars not visible due to non-task resource rows dominating chart

- Updated `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`:
  - Kept left resource panel showing filtered resources.
  - Changed right Syncfusion Gantt `DisplayResources` to only include resources referenced by current filtered tasks.
  - Result: task rows and chart rows align; bars are visible immediately instead of being pushed into non-visible lower rows.
- Diagnosis notes:
  - Snapshot check showed `Resources=119`, `Assignments=56`, and these assignments were all `resourceType=Vendor`.
  - Large number of non-task rows caused the visible viewport to initially land on empty rows.
- Validation:
  - `dotnet build MES/BlazorApp1/BlazorApp1/BlazorApp1.csproj -p:UseAppHost=false -p:OutDir=... -p:IntermediateOutputPath=...` passed (`0 errors`).

## Next steps (planned)

- Milestone 2 (next priority):
  - Add worktime background overlay (`ShowWorktimeBg`), window mask, freeze mask, and downtime blocks.
  - Make overlay react to scroll/zoom and keep `pointer-events:none`.
  - Improve `LinkMode=HoverOnly` behavior and reduce full refresh flicker.
- Milestone 3 (after milestone 2 stable):
  - Add taskbar setup-segment rendering (setup/run visual split).
  - Add context menu actions (highlight chain / clear / copy JSON / scroll to task start).
  - Add keyboard shortcuts (`F`, `+`, `-`, `Esc`) and optional drilldown interactions.
- Backend alignment suggestions:
  - Expose a dedicated `/api/gantt/syncfusion/snapshot` payload with stable resource tree/group fields and expanded calendars.
  - Return explicit `freezeEndTime`, richer task flags, and optional pre-built predecessor string for large datasets.

## 2026-02-04 - Gantt vendor split/fallback overlay stabilization (ongoing)

- Updated `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`:
  - Added `SplitVendorRowByOrder` workflow and virtual vendor row mapping to avoid all vendor tasks stacking into one row when backend only returns a single vendor resource key.
  - Removed `ExpandAllAsync` call path that triggered Syncfusion `NullReferenceException` during deselect/virtual expand; switched to JS-side safe expand helper.
  - Added fallback task overlay click selection and guarded selection logic (`TaskId <= 0` clears selection) to prevent accidental full dimming when group/empty rows are selected.
  - Changed group collapse behavior to re-apply filters immediately and synchronize visible resources with chart-side rows.
  - Added DOM row metric support (`ResourceRows`) and switched overlay Y-layout to real resource row positions from JS (`data-resource-id`) instead of index-based assumptions.
  - Added timeline-axis aware overlay math (`TimelineStart/TimelineEnd`) for interval/task rect X-position computation.
  - Refined fallback visibility gate to only show fallback when native taskbars are not detected.
- Updated `MES/BlazorApp1/BlazorApp1/wwwroot/js/gantt-asprova.js`:
  - Added scroll synchronization between left resource pane and right chart content.
  - Added row metric collection from left resource pane (`resourceRows: [{resourceId, top, height}]`).
  - Added timeline range capture from Syncfusion instance (`timelineStart`, `timelineEnd`).
  - Added `MutationObserver` hook on timeline area to emit viewport updates after zoom/timeline DOM changes.
  - Expanded native taskbar detection selector to `.e-taskbar-main-container, .e-gantt-child-taskbar`.
- Updated `MES/BlazorApp1/BlazorApp1/wwwroot/css/gantt-asprova.css`:
  - Enabled pointer interaction on fallback bars for direct selection (`pointer-events:auto`).
- Updated `MES/BlazorApp1/BlazorApp1/BlazorApp1.csproj`:
  - Strengthened exclusions for transient codex output/object folders (`_obj_codex*`, `_out_codex*`, nested `MES/BlazorApp1/...`) to avoid duplicate assembly attribute compile errors.
- Housekeeping:
  - Repeatedly cleaned accidental nested artifact folder `MES/BlazorApp1/BlazorApp1/MES` created by custom `OutDir` test builds.
- Current status:
  - Duplicate assembly attribute errors resolved after cleanup + csproj exclusions.
  - Syncfusion deselect/expand NRE no longer reproduced on the previous code path.
  - Vendor row split and overlay alignment improved, but user still reports zoom behavior mismatch in current session; further verification/fix is still in progress.

## 2026-02-06 - Syncfusion Gantt official-zoom route + stress UI + fit clipping fixes

- 09:00~10:00 时间轴与显示修复（官方样式回归）
  - 文件：`MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - 调整：
    - 统一按任务范围计算 `ProjectStartDate/ProjectEndDate`，并自动扩展到最早任务时间，避免左侧任务被裁切。
    - 底层时间刻度改为 24 小时格式（`HH:mm`）。
    - 增加加载后时间轴回到起点的滚动重置调用。
  - 文件：`MES/BlazorApp1/BlazorApp1/wwwroot/js/gantt-asprova.js`
    - 新增 `scrollTimelineToStart()`，确保每次刷新后 `scrollLeft=0`。

- 10:00~11:30 官方缩放方案落地（CustomZoomingLevels）
  - 文件：`MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - 调整：
    - 引入 `CustomZoomingLevels`（Month/Week/Day/Hour 分级），低倍不显示小时，高倍显示小时。
    - `ZoomIn/ZoomOut/ZoomToFit` 切回官方 API 调用（`SfGantt.ZoomInAsync/ZoomOutAsync/ZoomToFitAsync`）。
  - 文件：`MES/BlazorApp1/BlazorApp1/wwwroot/css/gantt-syncfusion.css`
    - 移除临时“小时文本强制隐藏”样式，避免和官方分级缩放冲突。

- 11:30~13:00 StressTest 前端开关与参数输入
  - 文件：`MES/BlazorApp1/BlazorApp1/Services/GanttApiClient.cs`
    - `GetSyncfusionResourceViewAsync` 增加 stress 参数透传：
      - `stressTest`
      - `stressResourceCount`
      - `stressTasksPerResource`
      - `stressSeed`
  - 文件：`MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
    - 新增页面控件：
      - `StressTest` 开关
      - `Stress Resources`
      - `Tasks / Resource`
      - `Stress Seed`
    - `Refresh` 时将 stress 参数传给后端 resource-view 接口。
    - 页面统计 `Source` 增加 `resource-view+stress` 标识，便于运行态识别数据来源。

- 13:00~14:30 ZoomToFit 右边缘裁切专项修复
  - 文件：`MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
    - 引入动态右侧缓冲：按可视宽度将像素缓冲换算为时间，动态修正 `ChartEnd`。
    - `ZoomToFit` 执行前先更新视口宽度，避免静态估算误差。
    - 增加“安全 Fit”逻辑：`ZoomToFit` 后若无横向余量则自动 `ZoomOut`，最多连续 3 次，减少最右任务圆角/箭头被裁切。
  - 文件：`MES/BlazorApp1/BlazorApp1/wwwroot/js/gantt-asprova.js`
    - 新增 `hasHorizontalScroll()`，用于判定是否存在横向余量。

- 验证
  - 多次执行：
    - `dotnet build MES/BlazorApp1/BlazorApp1/BlazorApp1.csproj -p:UseAppHost=false -p:OutDir=... -p:IntermediateOutputPath=...`
  - 结果：均通过（0 errors，存在既有 warnings）。

## 2026-02-07 - Syncfusion Gantt viewport/layout/task-detail interaction hardening

- 作用域：`frontend`
- 文件：`MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - 启用并保留虚拟化优化：
    - `EnableTimelineVirtualization="true"`
    - `EnableRowVirtualization="true"`
    - Stress 默认值调整为 `Resources=100`、`Tasks/Resource=20`，降低压力场景下连续加载等待。
  - 视口自适应高度：
    - 甘特高度改为 `Height="@GanttViewportHeight"`，首屏和刷新后根据可视区动态计算，避免底部滚动条悬空在页面中部。
  - 时间轴稳定显示策略（官方分级为主）：
    - `CustomZoomingLevels` 中将关键级别调整为 `Day/Day`，减少顶层日期与底层刻度错位感。
  - 任务条标签：
    - 增加 `GanttLabelSettings TaskLabel="TaskName"`，在色块上显示任务文本。
  - 右侧详情面板交互：
    - 详情面板改为按点击任务条触发显示；点击空白区隐藏。
    - 仅在有选中任务时渲染右侧详情卡片。
  - 布局空白修复：
    - 左侧甘特列改为动态宽度：无详情时 `col-12`，有详情时 `col-lg-9`。
    - 右侧 `col-lg-3` 仅在显示详情面板时渲染，消除“右侧大块空白”。

- 文件：`MES/BlazorApp1/BlazorApp1/wwwroot/js/gantt-asprova.js`
  - 增强宿主选择器，兼容 `.asprova-gantt-host` / `.syncfusion-gantt-wrap` / `.e-gantt`，避免事件未绑定。
  - 保留滚动与视口度量能力（`getMetrics`、滚动同步、MutationObserver）。
  - 详情面板触发改为更稳方案：
    - 绑定 EJ2 Gantt 原生 `taskbarClick`，从事件参数解析任务 `taskId` 回传 .NET。
    - 点击非任务条区域时回传 `taskId=0` 以隐藏详情面板。

- 文件：`MES/BlazorApp1/BlazorApp1/wwwroot/css/gantt-syncfusion.css`
  - 配合视口高度与右侧面板样式，维持详情卡片可读性与页面整体对齐。

- 验证
  - 多轮执行：
    - `dotnet build MES/BlazorApp1/BlazorApp1/BlazorApp1.csproj -p:UseAppHost=false -p:OutDir=... -p:IntermediateOutputPath=...`
  - 结果：通过（`0 errors`，存在既有 warnings）。

## 2026-02-07 - Syncfusion Gantt taskbar drag hardening (server-authoritative)

- 作用域：`frontend`
- 文件：`MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - 启用官方任务条拖拽编辑：
    - `GanttEditSettings AllowEditing="true" AllowTaskbarEditing="true"`
    - `GanttEvents TaskbarEdited="OnTaskbarEdited"`
  - 拖拽保存策略增强：
    - 纯平移优先提交 `DeltaMinutes`，非对称调整回退为 `NewStart/NewEnd`。
    - 新增 `AllowPush` 前端开关并透传后端，便于在约束冲突场景下提升成功率。
  - 用户反馈与容错：
    - 新增 `DragWarningMessage`，当后端返回“未产生有效移动”时给出警告，不再仅表现为“回原位”。
    - 对 `ApsApiException` 做规则化映射（锁定、最早/最晚窗口、资源冲突等），提供可读错误信息。
    - 保持“后端权威”模型：拖拽后始终重新拉取服务端结果，避免前端假状态。

- 说明
  - “拖完回原位”在当前 APS 约束体系下通常表示：后端拒绝变更或判定为 no-op；并非前端渲染丢失。

- 验证
  - `dotnet build MES/BlazorApp1/BlazorApp1/BlazorApp1.csproj -p:UseAppHost=false -p:OutDir=... -p:IntermediateOutputPath=...`
  - 结果：通过（`0 errors`，存在既有 warnings）。

## 2026-02-08 - Drag reject popup + global auto-reschedule (ortools) for Syncfusion Gantt

- 作用域：`frontend`
- 文件：`MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - 拖拽预拦截增强（`TaskbarEditing` / `TaskbarEdited`）：
    - 外协任务（`IsOutsourced`）禁止拖拽。
    - 锁定任务（`IsLocked`）禁止拖拽。
    - 压测模拟任务（`IsSimulated` 或 `SIM-WO-*`）禁止拖拽。
  - 失败反馈改进：
    - 拖拽被拒绝时，除页面 `ErrorMessage` 外，新增浏览器 `alert` 弹框，明确提示原因。
  - 成功后自动全局重排：
    - 单任务 `move` 成功且产生有效变化后，立即调用
      `POST /api/plans/{planId}/auto-schedule?engine=ortools&force=true`
      （通过 `ApsApi.AutoScheduleAsync(...)`）。
    - 状态栏展示重排结果（engine/scheduled/unscheduled）。
  - 保持后端权威刷新：拖拽后继续 `LoadDataAsync(clearMessages: false)`。

- 文件：`MES/BlazorApp1/BlazorApp1/Models/SyncfusionGanttModels.cs`
  - 新增任务字段：`IsSimulated`，用于前端识别并拦截压测模拟条。

## 2026-02-08 - Global reschedule confirmation dialog (estimated affected count)

- 作用域：`frontend`
- 文件：`MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - 在单任务 `move` 成功后、全局 `ortools` 重排前新增 `confirm` 弹窗：
    - 文案包含“预计影响任务数”（估算值：`max(1, pushedCount + 1)`）。
    - 用户确认后才执行全局自动重排。
    - 用户取消时保留本次单任务移动结果，并给出 `DragWarningMessage`。

## 2026-02-08 - Reschedule impact preview (taskId list) in confirm dialog

- 作用域：`frontend`
- 文件：`MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - 全局重排确认弹窗新增“预计影响任务 ID 预览”：
    - 基于 `move` 返回的 `TaskId + PushedTaskIds` 生成预览列表。
    - 超过 12 条时截断显示并追加 `(+N)` 提示。
  - 目的：用户确认前可直观看到可能受影响任务范围，降低误操作概率。

## 2026-02-08 - 左侧 TaskId 升序显示

- 作用域：`frontend`
- 文件：`MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - 任务数据在进入甘特前统一按 `TaskId` 升序排序（主数据分支 + fallback 分支）。
  - 目的：左侧任务编号展示更稳定，便于按 ID 定位与核对。

## 2026-02-07 - 今日补充汇总（Gantt 交互与稳定性）

- 作用域：`frontend`
- 文件：`MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - 视口与布局：甘特高度改为视口自适应；右侧详情面板隐藏时主视图自动占满（`col-12`），避免右侧空白。
  - 时间轴显示：采用更稳定的官方时间轴层级配置，减少顶部日期与底部刻度错位。
  - 任务标签：任务色块启用文本展示（`TaskLabel`），支持直接识别任务。
  - 详情面板交互：默认不显示；仅点击任务色块后显示；点击空白可关闭。
  - 拖拽编辑（官方）：启用 `AllowTaskbarEditing` + `TaskbarEdited` 事件。
  - 拖拽容错：
    - 纯平移优先 `DeltaMinutes`，否则回退 `NewStart/NewEnd`。
    - 新增 `AllowPush` 开关并透传后端。
    - 新增拖拽无效警告与后端约束错误映射，降低“拖完回原位但无提示”的排障成本。

- 文件：`MES/BlazorApp1/BlazorApp1/wwwroot/js/gantt-asprova.js`
  - 任务点击识别增强：优先接入 Syncfusion 原生任务条点击事件；补充宿主选择器兼容，避免事件绑定遗漏。
  - 保留滚动同步与视口度量逻辑，配合页面自适应与刷新后行为稳定。

- 稳定性说明
  - 当前采用“后端权威”模式：前端拖拽提交后以服务端结果为准再刷新，避免前端与排程引擎状态不一致。

- 验证
  - `dotnet build MES/BlazorApp1/BlazorApp1/BlazorApp1.csproj -p:UseAppHost=false -p:OutDir=... -p:IntermediateOutputPath=...`
  - 结果：通过（`0 errors`，存在既有 warnings）。
