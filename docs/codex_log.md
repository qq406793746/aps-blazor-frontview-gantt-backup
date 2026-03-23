## 2026-03-06 - 免费资源甘特 Demo（Frappe Gantt）

- 作用域：`frontend`
- 目标：按免费路线新增一个独立资源甘特演示页，覆盖“资源视图 + 依赖线 + 编辑 + 分页模拟虚拟滚动”基础能力，不影响现有 Syncfusion 页面。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttFreeResourceDemo.razor`
  - `MES/BlazorApp1/BlazorApp1/wwwroot/js/free-resource-gantt-demo.js`
  - `MES/BlazorApp1/BlazorApp1/wwwroot/css/site.css`
  - `MES/BlazorApp1/BlazorApp1/Pages/_Layout.cshtml`
  - `MES/BlazorApp1/BlazorApp1/Shared/NavMenu.razor`
  - `docs/codex_log.md`
- 页面入口：
  - 路由：`/gantt/free-resource-demo`
  - 左侧菜单：`免费资源甘特Demo`
- 如何验证：
  - 启动 BlazorApp1 后访问 `/gantt/free-resource-demo`
  - 切换 Day/Week/Month 视图，确认图表更新
  - 切换“允许拖拽编辑”，拖拽任务条后确认页面提示更新
  - 切换页码（上一页/下一页）确认资源分页与滚动正常
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

## 2026-02-09 GanttSyncfusion 详情栏与工具条交互修复

- 目标：解决任务详情栏关闭体验问题（需多次点击、关闭后空白/变窄、缩放状态被重置）并修复顶部工具条溢出。
- 改动时间：2026-02-09
- 改了哪些文件：
  - MES\BlazorApp1\BlazorApp1\Pages\GanttSyncfusion.razor
  - MES\BlazorApp1\BlazorApp1\wwwroot\css\gantt-syncfusion.css
  - MES\BlazorApp1\BlazorApp1\wwwroot\js\gantt-asprova.js
  - C:\Users\123\.codex\skills\safe-minimal-change-research\references\变更前必查清单.md
- 关键改动：
  - 详情栏增加“关闭”按钮；关闭后保持手动关闭状态，避免被同一选中事件立即重开。
  - 将任务详情改为右上角浮层（overlay），不再占据主栅格宽度；主甘特图区保持全宽，避免关闭后右侧留白。
  - 移除临时调试文案 SelectedTaskId=...。
  - 顶部按钮组改为 btn-sm + flex-wrap，修复侧边栏弹出时 ZoomToFit 溢出到白板外的问题。
- 如何验证：
  - 访问 /gantt/syncfusion，点击任务条，确认详情栏出现并可单击关闭。
  - 关闭详情后，确认甘特图不变窄、无右侧空白，当前 zoom/滚动状态保持。
  - 展开左侧导航，确认 Refresh/Latest Plan/ZoomIn/ZoomOut/ZoomToFit 不溢出。
- 说明：
  - 中途尝试过 RefreshChartElement，当前包版本不可直接调用，已移除，最终采用“浮层不占布局宽度”的稳定方案。

## 2026-02-09 - 待办清单：Gantt 体验对齐 Asprova（前端）

- 作用域：`frontend`
- 背景：当前已完成选中任务自动定位、详情栏可关闭、关闭后不重置初始状态；仍有可用性提升空间。
- 待完成功能（前端可独立推进）：
  - 工具栏自适应优化：侧边导航展开时，按钮不换行错位、不溢出容器。
  - 缩放与焦点一致性：`ZoomIn/ZoomOut/ZoomToFit` 后持续保持“当前选中任务可见且高亮”。
  - 详情栏交互一致性：关闭后保持关闭，只有用户明确重新选中任务时才再次打开。
  - 时间轴可读性：按缩放级别优化刻度密度与标签显示，减少空白和拥挤切换感。
  - 任务状态可视化：对锁定/外协/模拟任务增加统一图例与样式说明。
- 验证建议：
  - 连续执行：选中任务 -> 缩放 -> 拖动 -> 关闭详情栏 -> 再缩放，确认焦点与布局稳定。

## 2026-03-04 - Syncfusion 资源甘特汉化与交互稳健性补强（前端）

- 目标：
  - 完成 Syncfusion 资源甘特页面中文化与任务名可读化；
  - 增加拖拽前预判与业务化冲突提示；
  - 替换原生 alert/confirm，降低交互生硬感；
  - 将压测入口与生产操作分离；
  - 缩放命令识别去文案依赖，避免多语言后失效。

- 修改时间：2026-03-04 17:01

- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
    - 顶部与表格文案汉化（含 `ID/Name/Start Date/End Date/Duration/Dependency`）。
    - 新增“高级模式”开关，`StressTest` 仅在高级模式展示。
    - 任务拖拽前新增本地可行性预判（时间窗/资源重叠），不通过则直接阻止拖拽。
    - `MapMoveError` 改为业务化提示语。
    - 新增页面内统一弹窗 `UiDialog`，替代浏览器原生 `alert/confirm`。
    - 前端任务名规则调整：优先使用后端 `DisplayName` 原样展示；兜底规则保留工单号，避免“只显示第10道工序”。
  - `MES/BlazorApp1/BlazorApp1/wwwroot/js/gantt-asprova.js`
    - 缩放命令识别改为优先 `data-zoom-cmd` 与类名（`e-zoomin/e-zoomout/e-zoomtofit`），文本匹配仅作降级兜底。
  - `MES/BlazorApp1/BlazorApp1/wwwroot/css/gantt-syncfusion.css`
    - 新增页面内弹窗样式（含明暗主题兼容）。
  - `MES/BlazorApp1/BlazorApp1/Models/SyncfusionGanttModels.cs`
    - `GanttTaskVm` 新增 `DisplayName` 字段映射（`[JsonPropertyName("displayName")]`）。

- 如何验证：
  - 访问 `/gantt/syncfusion`，确认页面控件与列头为中文。
  - 默认模式下确认不展示 `StressTest`；开启“高级模式”后出现压测参数。
  - 拖拽任务到明显冲突时，确认拖拽被提前拦截并提示“预判不可行”。
  - 触发拖拽失败/全局重排确认时，确认使用页面弹窗而非浏览器原生弹窗。
  - 点击放大/缩小/适应窗口，确认缩放正常（不依赖按钮文案）。
  - 刷新数据后抽查任务名称，确认优先显示后端 `displayName`，不再退化为纯“第10道工序”。

- 未解决事项：
  - `BlazorApp1` 本地构建仍可能被运行中进程锁定（`BlazorApp1.exe/.dll`），需先停止占用进程再完整构建。
  - 拖拽前预判当前为前端本地规则，尚非后端权威预检接口；复杂约束仍以后端 `move` 校验为准。


## 2026-03-06 - 免费资源甘特页（/gantt/free-resource-demo）修复记录（防乱码）

### 背景
- 目标：去掉商业组件试用限制后，提供免费路线的资源甘特展示，并尽量贴近 `/gantt/syncfusion` 页面体验。
- 问题：前期 Frappe 路线出现“时间轴可见但任务条挤在一起/不可见”，且多次出现中文显示异常。

### 今日实际改动
1. 许可证提示处理（后端）
- 在 `Program.cs` 完成 Syncfusion License 注册接入（用于消除试用弹窗）。

2. 免费 Demo 页面与路由接入（前端）
- 新增页面：`Pages/GanttFreeResourceDemo.razor`（路由：`/gantt/free-resource-demo`）。
- 新增菜单入口：`Shared/NavMenu.razor`（“免费资源甘特Demo”）。
- 在布局注入脚本：`Pages/_Layout.cshtml` 引入 `/js/free-resource-gantt-demo.js`。

3. Frappe 路线调试（已实践但效果不稳定）
- 新增脚本：`wwwroot/js/free-resource-gantt-demo.js`。
- 新增样式：`wwwroot/css/site.css` 中 `.free-rg-*` 及可见性兜底样式。
- 针对“任务条丢失”做过日期对象转换、容器高度、强制可见等修复。

4. 最终切换方案（当前生效）
- `/gantt/free-resource-demo` 改为复用项目内稳定的 `Components/Gantt/ResourceGantt.razor` 渲染内核。
- 保留 Syncfusion 风格工具栏交互（刷新、最新计划、缩放、分页、编辑开关）。
- 将容器从 `classic-gantt-panel` 切换为 `syncfusion-gantt-wrap free-rg-sync-panel`，避免经典绿底风格覆盖。
- 在 `site.css` 新增 `free-rg-sync-panel` 专属皮肤：
  - 隐藏顶部调试统计行；
  - 时间轴改浅灰风格；
  - 资源区白底；
  - 任务条改为蓝色风格；
  - 依赖线、规划线颜色调整为接近 Syncfusion 视觉。

### 关键文件
- `MES/BlazorApp1/BlazorApp1/Pages/GanttFreeResourceDemo.razor`
- `MES/BlazorApp1/BlazorApp1/wwwroot/js/free-resource-gantt-demo.js`
- `MES/BlazorApp1/BlazorApp1/wwwroot/css/site.css`
- `MES/BlazorApp1/BlazorApp1/Pages/_Layout.cshtml`
- `MES/BlazorApp1/BlazorApp1/Shared/NavMenu.razor`

### 构建与运行
- 多次 `dotnet build` 期间出现 `BlazorApp1.exe` 被占用（常见 PID：8400、17836、21484）。
- 处理方式：结束占用进程后重建，最终构建成功（0 error）。

### 防乱码约定（新增）
- 本仓库涉及中文文案的文件统一使用 `UTF-8`（建议 `UTF-8 without BOM`）保存。
- 通过 PowerShell 写文件时显式指定编码：`-Encoding UTF8`。
- 若页面出现中文异常，优先检查：
  - 文件编码是否被工具改写；
  - 浏览器是否命中旧缓存（强制刷新 `Ctrl+F5`）；
  - 是否混入历史乱码文本片段。

## 2026-03-12 - Syncfusion 甘特拖拽重排差异高亮与结果可视化（前端）

- 作用域：`frontend`
- 目标：
  - 解决“拖动任务后已触发全局自动重排，但甘特图前后变化不明显、用户难以判断哪些任务真的变了”的问题；
  - 在不改后端接口协议的前提下，用最小改动增强前端的重排结果可视化。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `MES/BlazorApp1/BlazorApp1/wwwroot/css/gantt-syncfusion.css`
- 实施思路：
  - 在前端执行全局自动重排前，先缓存当前任务快照（`TaskId -> StartDate / EndDate / ResourceIds`）；
  - 自动重排完成并重新加载数据后，对比前后任务快照；
  - 将“手工移动的任务”和“自动重排后发生变化的任务”写入不同的前端高亮集合；
  - 通过 Syncfusion 官方支持的 `QueryChartRowInfo` 事件给对应任务行追加 CSS class，再由样式控制任务条颜色；
  - 同时在页面消息区直接输出：
    - `手工移动任务ID`
    - `自动重排变化任务ID`
  - 在甘特图区右上角增加小图例，说明颜色含义。
- 当前可视化规则：
  - 橙色：手工移动任务
  - 绿色：自动重排后发生变化的任务
  - 虚线边框：锁定任务
- 预期效果：
  - 用户执行拖拽并确认全局自动重排后，不再只能看“已排/未排统计”；
  - 可以直接从甘特图颜色和变化任务 ID 文本中判断：
    - 哪条任务是人工调整的；
    - 哪些任务是系统自动重排联动变化的；
    - 如果只有橙色、没有绿色，则说明本次自动重排对其他任务影响较小，不是前端未刷新。
- 验证建议：
  - 打开 `/gantt/syncfusion`，加载一个可编辑计划（如 `PlanId=10096`）；
  - 拖动一个可成功移动的任务并确认执行全局自动重排；
  - 观察：
    - 甘特图区右上角图例是否出现；
    - 手工移动任务是否变为橙色；
    - 自动重排后发生变化的其他任务是否变为绿色；
    - 页面消息区是否列出本次变化任务 ID；
  - 手动点击“刷新”后，差异高亮与变化提示会清空，便于进入下一轮演示。
- 备注：
  - 当前方案完全基于前端快照比对，不改后端 `PlanAutoScheduleResult` 返回结构；
  - 本地 `dotnet build` 仍可能因运行中的 `BlazorApp1.exe/.dll` 或 Visual Studio 锁文件失败，若需完整构建验证需先停止占用进程。

## 2026-03-12 - React 替代评估与 Syncfusion 许可讨论记录

- 作用域：`frontend`
- 背景：
  - 讨论是否将当前 Blazor + Syncfusion 甘特页改为 React 技术栈；
  - 同时评估 React 免费开源甘特方案是否能覆盖现有 APS 页面能力；
  - 进一步确认 Syncfusion 在企业内部自用场景下的许可路径与风险。

### 1. React 替代当前甘特页的可行性判断

- 当前前端并非 React，而是 Blazor：
  - 现有核心页面为 `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - 项目类型为 `Microsoft.NET.Sdk.Web`
- 结论：
  - 技术上可以改用 React；
  - 但这不是“平滑切换框架”，而是前端甘特页的重做；
  - 后端 API 可以复用，前端页面层、组件层和交互逻辑需要重写。

### 2. React 免费开源甘特方案调研结论

- 调研过的候选：
  - `SVAR React Gantt`
  - `@jaeungkim/gantt-chart`
  - `DHTMLX Gantt` 开源版
  - `Frappe Gantt` React wrapper
- 结论：
  - 如果只看“普通项目甘特图能力”，`SVAR React Gantt` 是相对更合适的 React 开源候选；
  - 但如果对标当前 `/gantt/syncfusion` 的 APS 资源甘特页面，尤其是：
    - 资源视图
    - 多资源分配
    - 拖拽前后业务校验
    - 自动重排联动
    - 差异高亮与结果解释
    则无法 1:1 替代。
- 评估结论：
  - 仅从甘特主体能力看，`SVAR React Gantt` 约可达到现页面 `50%-65%`
  - 若要求保住当前 APS 资源排产交互，现实覆盖度更接近 `35%-50%`
- 因此：
  - 不建议为了当前这页直接切 React；
  - 如确需验证 React 路线，应先做独立 PoC 页面，而不是直接替换现有页面。

### 3. Syncfusion 商业许可与 Community License 判断

- 商业许可：
  - 官方当前主推 `Team License`
  - 公开价格参考：
    - `up to 5 developers`: `$395/月`
    - 约 `$4740/年`
  - 许可是按可接触 Syncfusion assemblies 的开发者计，不是只算单个页面开发者。
- Community License 资格要求（官方口径）：
  - 年营收 `< 100 万美元`
  - 开发者 `<= 5`
  - 总员工 `<= 10`
  - 外部融资累计 `<= 300 万美元`
  - 不能是政府或政府相关组织
- 对 `UM / UNITED MACHINING SOLUTIONS` 的判断：
  - 若以公司主体申请或实际为公司内部系统正式使用，基本不符合 Community License；
  - 即便只有 `2` 个开发者，营收与员工规模也远超社区许可门槛。

### 4. 中国大陆个人申请 Community License 讨论结论

- 官方能确认的审核要点：
  - 需填写 `Community License validation form`
  - 官方会创建验证 ticket
  - 明确要求 `LinkedIn or Xing profile`
- 公开经验层面：
  - 中国大陆个人申请存在成功案例；
  - 但没有可信的“通过率”统计数据；
  - 个别经验提到在无法方便提供 `LinkedIn/Xing` 时，可在 ticket 中补充说明并提供其他个人主页辅助审核。
- 关键合规判断：
  - 若项目实际归属于不符合社区许可资格的大公司或客户，个人名义申请并不能稳妥覆盖正式企业内部使用场景；
  - 因此不建议将个人 Community License 作为 UM 内部正式项目的长期许可方案。

### 5. 成本与路线结论

- 在当前项目阶段，对比：
  - 方案 A：继续沿用 Syncfusion
  - 方案 B：重做 React 甘特页
- 结论：
  - 对现有 APS 项目，继续用 Syncfusion 更划算；
  - 原因不是单看 license 价格，而是当前前端已经沉淀了较多 APS 特有交互：
    - 资源视图
    - 拖拽校验
    - 自动重排触发
    - 差异高亮
    - 变化任务 ID 提示
  - 若改 React，需要重写的并不是“甘特图皮肤”，而是整套排产交互层。

### 6. 当前建议

- 正式企业内部使用：
  - 优先按商业 `Team License` 路线评估
- React 技术路线：
  - 如要探索，只建议先做独立 PoC
  - 不建议直接替换现有 `/gantt/syncfusion`

## 2026-03-12 - 今日前端实际工作与技术路线结论补充

- 作用域：`frontend`

### 1. 今日实际完成的页面改动

- 本次实际修改文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `MES/BlazorApp1/BlazorApp1/wwwroot/css/gantt-syncfusion.css`
  - `docs/codex_log.md`
- 在 `GanttSyncfusion.razor` 中补了以下具体逻辑：
  - 给 `SfGantt` 增加 `QueryChartRowInfo="OnQueryChartRowInfo"`，通过 Syncfusion 官方事件把前端计算出的 CSS class 挂到图表行；
  - 新增快照与高亮状态字段：
    - `LastScheduleSnapshot`
    - `RecentlyMovedTaskIds`
    - `RecentlyRescheduledTaskIds`
  - 新增前后快照比对方法：
    - `CaptureTaskSnapshot(...)`
    - `ApplyRescheduleDiffMarkers(...)`
    - `BuildRescheduleDiffMessage(...)`
    - `BuildTaskCssClass(...)`
    - `ApplyTaskVisualMarkers(...)`
  - 在 `OnTaskbarEdited(...)` 中调整执行顺序：
    - 先记录自动重排前任务快照；
    - 拖拽成功后若用户确认全局自动重排，则调用 `AutoScheduleAsync(...)`；
    - 自动重排完成后重新 `LoadDataAsync(...)`；
    - 再用前后快照比对出发生变化的任务；
    - 最后把差异任务高亮并输出任务 ID 提示。
  - 在 `TryApplyLocalMoveResult(...)` 中补了本地回写后的视觉反馈：
    - 当前任务本地回写成功后立即标记为橙色；
    - 同时写入 `RescheduleDiffMessage = 手工移动任务ID...；尚未执行全局自动重排。`
  - 在消息区新增一个专门的文本输出区：
    - `RescheduleDiffMessage`
    - 用来直接展示：
      - `手工移动任务ID`
      - `自动重排变化任务ID`
- 在 `gantt-syncfusion.css` 中补了以下具体样式：
  - 新增右上角悬浮图例：
    - `.syncfusion-legend`
    - `.syncfusion-legend-chip-moved`
    - `.syncfusion-legend-chip-rescheduled`
  - 新增任务条差异高亮样式：
    - `.sync-taskbar-moved`
    - `.sync-taskbar-rescheduled`
    - `.sync-taskbar-moved.sync-taskbar-rescheduled`
    - `.sync-taskbar-locked`
- 本次页面上的最终可见效果为：
  - 橙色：用户手工拖动的任务
  - 绿色：自动重排后开始/结束时间或资源发生变化的任务
  - 虚线边框：已锁定任务
  - 右上角固定图例说明颜色含义
  - 页面消息区直接打印变化任务 ID 列表
- 本次改动延续并配套了此前已做的拖拽增强：
  - 英文错误消息业务化中文提示；
  - 拖拽失败后重新加载并尝试重新定位原任务；
  - 单任务局部移动时本地回写并锁定，避免整页刷新导致用户丢失当前任务位置。

### 2. 今日验证与使用反馈

- 今日用于实际页面验证的主样例：
  - `PlanId = 10096`
  - `TaskId = 11899`
- 选择该任务的原因：
  - 先前尝试的 `11922`、`11939-11942` 等路线存在明显“同一人员冲突”问题，白天拖动时容易先被 `Person conflict` 拦截；
  - `11899` 属于 `WorkItemId = 10135` 的首道工序，且对应后续链路 `11900 / 11901`，更适合观察手工干预后自动重排是否真的联动。
- 实际观察到的页面行为：
  - 手工拖动 `11899` 成功后，页面先提示：
    - `任务 11899 已移动。全局重排引擎=ortools，已排=152，未排=0`
  - 页面随后直接输出：
    - `手工移动任务ID：[11899]`
    - `自动重排变化任务ID：[11803, 11804, ... 11954]`
  - 甘特图区中：
    - `11899` 对应任务条显示为橙色；
    - 自动重排后受影响的任务条显示为绿色；
    - 右上角图例同步标明颜色含义。
- 这次验证带来的结论比之前更明确：
  - 之前“自动重排看起来变化不明显”，并不一定是接口没执行；
  - 很大一部分原因是旧页面没有把“哪些任务变了”可视化出来；
  - 现在补上颜色和任务 ID 提示后，能够直接判断：
    - 是系统真的没怎么动；
    - 还是系统动了很多条，只是过去不容易肉眼识别。
- 当前保留策略：
  - 差异高亮不会自动消失；
  - 会一直保留到用户下一次手动点击“刷新”为止；
  - 这样更适合演示、截图和现场汇报。

### 3. 关于 Element Plus / React / 桌面端替代路线的今日讨论结论

- 用户提出是否可改为 `React`、`Element Plus`、`WinForms + SunnyUI`、或 `WPF`。
- 今日结论：
  - 当前 `/gantt/syncfusion` 页面已不仅仅是“甘特图展示”，而是一页包含 APS 资源视图、拖拽校验、后端 `move`、自动重排、差异高亮与结果解释的复杂页面；
  - 因此无论改 `React` 还是 `Element Plus + 第三方甘特`，都不是简单替换 UI，而是重写整页交互逻辑；
  - `Element Plus` 本身不是甘特图库，只能作为外围 UI 壳子；
  - `SunnyUI` 适合 WinForms 美化，但不适合当前 Web 路线，也不能替代甘特核心能力；
  - `WPF` 技术上可行，但对当前项目阶段不划算。
- 综合判断：
  - 现阶段最合理的是继续沿用 `Blazor + Syncfusion`；
  - 若担心 Syncfusion 许可无法落地，可将：
    - `React + 开源甘特`
    - 或 `Vue + Element Plus + 开源甘特`
    作为备份 PoC 路线，而不是立刻替换正式页面。

### 4. 关于“现有 Blazor 逻辑能否照着重写到 Element Plus / React”的结论

- 今日明确过一个重要判断：
  - 当前 Blazor 页面已经沉淀了比较完整的业务逻辑与交互流程；
  - 这能显著降低新技术栈下的需求梳理成本；
  - 但并不能直接降低组件级重写成本。
- 换言之：
  - 可以“照着再写一遍”
  - 但这不等于“迁移成本很低”
  - 当前代码更像是“完整施工图”，不是“能直接搬过去的预制件”。
- 今日明确提到的“能照着写，但不能直接搬”的具体逻辑包括：
  - 任务拖拽前预判：`OnTaskbarEditing(...)`
  - 任务拖拽后保存与自动重排：`OnTaskbarEdited(...)`
  - 任务行高亮挂载：`OnQueryChartRowInfo(...)`
  - 失败后重新定位：`ReloadAndRefocusTaskAsync(...)`
  - 手工移动后本地回写与锁定：`TryApplyLocalMoveResult(...)`
  - 这些逻辑的业务判断可以复用，但在 `Element Plus` 或 `React` 甘特库里需要按照新事件模型重写。

### 5. 当前推荐策略（截至今日）

- 主线：
  - 继续推进当前 `Blazor + Syncfusion` 甘特页，优先把现有 APS 页面做强做稳
- 备线：
  - 如需防范 Syncfusion 许可风险，可单独准备一个最小 PoC
  - 优先级建议：
    - `React + 开源甘特` 作为首选备份路线
    - `Vue + Element Plus + 开源甘特` 作为次选
- 不建议作为当前备份路线的方向：
  - `WinForms + SunnyUI`
  - `WPF`

## 2026-03-12 - Syncfusion 甘特手动“自动排程”入口改造（前端）

- 作用域：`frontend`
- 背景：
  - 当前 `/gantt/syncfusion` 的全局自动重排主要绑定在“任务拖拽成功后”的确认流程上；
  - 这会让“系统导入了新订单/加急订单后，用户希望直接看重排结果”的主流程不够自然，容易误导成“必须先拖一下任务才能触发重排”。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/codex_log.md`
- 实施内容：
  - 在 `/gantt/syncfusion` 顶部工具栏新增手动按钮：`自动排程`。
  - 新增前端方法 `RunAutoScheduleAsync()`：
    - 直接调用既有 `POST /api/plans/{planId}/auto-schedule?engine=ortools&force=true`
    - 复用当前页面时间窗 `StartInput / EndInput`
    - 调度完成后自动重新加载甘特数据
    - 继续复用前端现有的“前后快照比对 + 差异高亮 + 变化任务ID提示”机制
  - 调整 `OnTaskbarEdited(...)`：
    - 拖拽成功后不再弹“是否立即执行全局自动重排”确认框
    - 改为仅保存当前任务移动结果，并提示用户如需系统整体重排，请手动点击上方 `自动排程`
- 新的推荐使用逻辑：
  - 正常手工微调：拖拽任务，仅保存局部移动
  - 新订单/加急订单导入后：用户手动点击 `自动排程`
  - 自动排程完成后：前端直接展示重排后的甘特结果，并高亮发生变化的任务
- 验证建议：
  - 访问 `/gantt/syncfusion`，确认顶部出现 `自动排程` 按钮
  - 导入新订单或切换到含急单的 `PlanId` 后，直接点击 `自动排程`
  - 确认页面无需先拖拽任务，也能刷新为新的排程结果
  - 若本次重排引起任务时间/资源变化，确认页面仍会显示变化任务 ID 与绿色差异高亮

## 2026-03-12 - 自动排程按钮点击无感知修复（前端）

- 作用域：`frontend`
- 背景：
  - 手动新增的 `自动排程` 按钮虽然已绑定后端调用，但点击后在请求完成前缺少明确的前端反馈；
  - 用户容易感知为“按钮没反应”。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/codex_log.md`
- 实施内容：
  - 问题现象：
    - 页面上已经出现 `自动排程` 按钮，但点击后在接口返回前没有即时提示；
    - 当后端执行时间稍长时，用户主观感受接近“点击无反应”。
  - 前端处理：
    - `RunAutoScheduleAsync()` 在请求发起前立即写入状态提示：
      - `正在执行自动排程，请稍候...`
    - 若页面当前仍在加载，改为明确提示：
      - `当前仍在加载或执行其他操作，请稍后再试自动排程。`
    - 若自动排程接口失败，除页面错误文本外，同时弹出统一页面内提示框，避免失败被忽略。
  - 目的：
    - 让“按钮已触发”“系统正在排程”“排程失败”三个状态在页面上可区分，不再只靠最终结果判断。
- 验证建议：
  - 打开 `/gantt/syncfusion`，点击 `自动排程`
  - 确认点击后会立即出现“正在执行自动排程，请稍候...”
  - 若后端失败，确认页面会弹出错误提示，而不是静默无反馈

## 2026-03-12 - 自动排程按钮禁用表达式修复（前端）

- 作用域：`frontend`
- 背景：
  - 页面实际输出中，自动排程按钮被渲染为 `disabled="False || PlanId <= 0"`；
  - 浏览器只要看到 `disabled` 属性，就会把按钮当成禁用状态，导致点击完全无反应。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/codex_log.md`
- 处理：
  - 问题根因：
    - 组件里原先写成 `disabled="@Loading || PlanId <= 0"`；
    - 该写法没有把整个表达式包成一个 Razor 求值单元，最终输出到了 HTML 字面量；
    - 页面实际被渲染成了类似 `disabled="False || PlanId <= 0"` 的结果。
  - 为什么会导致“完全没反应”：
    - 对浏览器来说，只要按钮存在 `disabled` 属性，不管属性值写成什么字符串，按钮都会被当成禁用态；
    - 所以前端事件根本不会触发，表现就是点击完全无效。
  - 修复方式：
    - 将按钮属性从 `disabled="@Loading || PlanId <= 0"` 改为 `disabled="@(Loading || PlanId <= 0)"`；
    - 明确让 Razor 先计算布尔表达式，再按最终 `true/false` 输出属性。
  - 修复后的预期：
    - 仅当页面正在加载，或 `PlanId <= 0` 时按钮禁用；
    - 正常计划场景下按钮应恢复为可点击状态。
- 验证建议：
  - 强刷 `/gantt/syncfusion`
  - 确认 `PlanId > 0` 且页面未加载时，按钮可点击
  - 点击后应立即出现“正在执行自动排程，请稍候...”

## 2026-03-12 - 自动排程后右侧白板区域修复（前端）

- 作用域：`frontend`
- 背景：
  - 在手动点击 `自动排程` 后，Syncfusion 甘特右侧偶发出现“白色空白区域”，原有网格背景没有覆盖到整个可视滚动区；
  - 该现象更像图表重绘/尺寸刷新不完整，而不是数据缺失。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `MES/BlazorApp1/BlazorApp1/wwwroot/css/gantt-syncfusion.css`
  - `docs/codex_log.md`
- 处理：
  - 现象说明：
    - 自动排程完成后，左侧任务和已有任务条能正常刷新；
    - 但时间轴右侧靠近后续日期的位置，偶发出现一整块白色区域；
    - 原本应存在的行网格背景没有铺满该区域，视觉上像“图表断了一截”。
  - 判断依据：
    - 该区域并非数据缺失，因为任务列表和时间轴本身仍然存在；
    - 更接近自动排程后图表宽度、滚动区或重绘节奏没有完全同步。
  - 前端修复：
    - 在 `RunAutoScheduleAsync()` 完成数据重载与差异高亮后，额外调用：
      - `RefitChartRangeToViewportAsync()`
      - `NotifyResizeStableAsync()`
    - 第一项用于重新贴合当前视口和时间轴范围；
    - 第二项用于补发稳定的尺寸刷新通知，促使 Syncfusion 图表完成一次完整重排。
  - 样式兜底：
    - 给 `.e-chart-root-container` 与 `.e-chart-scroll-container` 补基础横向网格底纹；
    - 即使某次组件内部没有及时把背景重新绘满，右侧空白区也会保持网格视觉，不再直接显示纯白底。
  - 预期效果：
    - 自动排程后，右侧即使没有任务条，也仍然维持连续的网格背景；
    - 视觉上与左侧任务区保持一致，不再出现明显断层。
- 验证建议：
  - 打开 `/gantt/syncfusion`
  - 点击 `自动排程`
- 观察右侧空白区是否仍出现纯白板
- 确认即使右侧没有任务条，背景仍保持网格感而不是整块白底

## 2026-03-16 - Syncfusion 任务时间诊断与首次进入对齐修复

- 作用域：`frontend`
- 背景：
  - `/gantt/syncfusion` 在 `PlanId=10096` 下，首次进入页面时，任务详情面板中的 `开始/结束` 与后端真实任务时间不一致；
  - 手动点击页面右上角 `刷新` 后，详情时间又会恢复正确；
  - 说明问题不在后端排程结果，而在前端首次加载后的页面状态/展示链路。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/playbook/progress.txt`
  - `docs/playbook/lessons.md`
  - `docs/codex_log.md`
- 处理：
  - 在任务详情面板中新增：
    - `后端开始`
    - `后端结束`
    - `时间一致性`
    - `真值诊断`
  - 选中任务时，页面会调用现有 `api/plans/{planId}/tasks` 拉取该任务的后端真值；
  - 读取到真值后，直接覆盖当前 `SelectedTask.StartDate / EndDate / DurationValue`，并同步修正 `DisplayTasks` 中对应任务；
  - 额外在 `LoadDataAsync()` 完成后，引入一次按 `TaskId` 的后端真值校正，尽量缩小首次进入与手动刷新的差异。
- 验证计划：
  - 打开 `/gantt/syncfusion`
  - 输入 `PlanId=10096`
  - 不手动点刷新，直接点击任务 `12115`
  - 确认：
    - `开始/结束` 与 `后端开始/后端结束` 一致
    - `时间一致性` 显示 `一致`

## 2026-03-16 - Syncfusion 自定义任务条模板拖拽命中修复

- 作用域：`frontend`
- 背景：
  - 页面改为使用 `TaskbarTemplate` 做任务条差异高亮后，资源视图下部分任务出现“完全拖不动”的现象；
  - 在 `PlanId=10096` 下，`12122 / 12155` 已确认不是锁定任务，也不是外协任务；
  - 打开 `允许连带推移` 后仍然完全不能拖动，更像组件原生拖拽命中被模板 DOM 覆盖，而不是业务约束拦截。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/wwwroot/css/gantt-syncfusion.css`
  - `docs/playbook/progress.txt`
  - `docs/playbook/lessons.md`
  - `docs/codex_log.md`
- 处理：
  - 给 `.sync-taskbar-inner` 增加：
    - `pointer-events: none`
    - `user-select: none`
  - 给 `.sync-taskbar-label` 增加：
    - `pointer-events: none`
  - 目的：
    - 让鼠标拖拽命中回到 Syncfusion 原生 taskbar 层，而不是落在自定义模板内部 DOM 上；
    - 保留模板视觉高亮，不再阻断拖拽手势。
- 验证计划：
  - 打开 `/gantt/syncfusion`
  - 输入 `PlanId=10096`
  - 在资源视图下勾选 `允许连带推移`
  - 回归拖拽：
    - `12122`
    - `12155`
  - 确认任务条至少可以被拖动进入编辑/保存流程，而不是完全无响应

## 2026-03-16 - Syncfusion 拖拽起手与详情刷新解耦

- 作用域：`frontend`
- 背景：
  - 在完成首次进入时间对齐和模板命中修复后，`PlanId=10096` 下 `12122 / 12155` 仍反馈“完全拖不动”；
  - 继续审查页面事件链后发现，`RowSelected` 和自定义 `OnGanttTaskbarClicked` 会在任务条点击/选中时立即触发后端真值读取，并调用 `StateHasChanged`；
  - 这类刷新发生在拖拽起手阶段时，可能直接打断 Syncfusion 的拖拽手势，即使任务本身并未被锁定。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/playbook/progress.txt`
  - `docs/playbook/lessons.md`
  - `docs/codex_log.md`
- 处理：
  - 保留任务选中和详情面板展示；
  - 把 `RowSelected` / `OnGanttTaskbarClicked` 中原本同步执行的后端真值刷新改成延迟后台执行；
  - 新增 `RefreshSelectedTaskServerTruthSafeAsync(...)`：
    - 先等待短暂延迟，避开拖拽起手；
    - 若页面仍在 `Loading` 或已进入 `IsApplyingTaskbarMove`，则跳过此次诊断刷新；
    - 仅在后台补充详情诊断，不再把拖拽体验绑死在选中刷新上。
- 预期：
  - 任务条拖拽起手不再被详情诊断刷新打断；
  - 点击任务后，详情真值仍会在短延迟后补齐。
- 验证计划：
  - 重启 `BlazorApp1`
  - 打开 `/gantt/syncfusion`
  - 输入 `PlanId=10096`
  - 在资源视图下勾选 `允许连带推移`
  - 直接回归拖拽：
    - `12122`
    - `12155`
  - 确认：
    - 任务条至少可以进入拖拽编辑态；
    - 详情面板仍能在稍后显示后端真值

## 2026-03-16 - Syncfusion TaskbarTemplate 恢复原生任务条外壳类

- 作用域：`frontend`
- 背景：
  - 即使已经处理模板命中层和详情刷新打断问题，`PlanId=10096` 下 `12122 / 12155` 仍反馈“完全拖不动”；
  - 进一步对照 Syncfusion 官方文档和论坛说明，发现 `TaskbarTemplate` 如果完全用自定义 DOM 替代默认任务条外壳，而没有保留组件识别拖拽所需的默认 taskbar 类，任务条会显示出来，但拖拽/缩放能力会丢失。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `MES/BlazorApp1/BlazorApp1/wwwroot/css/gantt-syncfusion.css`
  - `docs/playbook/lessons.md`
  - `docs/codex_log.md`
- 处理：
  - 调整 `TaskbarTemplate` 结构：
    - 模板根节点恢复为带官方任务条类的外壳；
    - 我们自己的彩色高亮样式只放到内层 `sync-taskbar-inner`；
    - 标签补上 `e-task-label`；
  - 目的：
    - 保留自定义颜色和文字；
    - 同时把拖拽命中、缩放和编辑控制权交还给 Syncfusion 原生 taskbar 容器。
- 验证计划：
  - 重启 `BlazorApp1`
  - 打开 `/gantt/syncfusion`
  - 输入 `PlanId=10096`
  - 勾选 `允许连带推移`
  - 回归拖拽：
    - `12122`
    - `12155`
  - 确认至少能进入拖拽编辑态，而不是完全无响应

## 2026-03-16 - 修复手工拖拽一次后任务被前端误锁定

- 作用域：`frontend`
- 背景：
  - 任务条恢复可拖拽后，又出现了“第一次能拖，第二次立刻不能再拖”的现象；
  - 继续检查本地回写逻辑后发现，拖拽成功进入 `TryApplyLocalMoveResult(...)` 时，前端会把当前任务直接改成 `IsLocked = true`；
  - 页面拖拽预检查 `TryGetDragRejectReason(...)` 又会把 `IsLocked` 任务直接拦掉，于是形成“手工拖一次后，被前端自己锁死”的错误行为。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/playbook/lessons.md`
  - `docs/codex_log.md`
- 处理：
  - 删除 `TryApplyLocalMoveResult(...)` 里的 `task.IsLocked = true` 本地回写；
  - 保留“手工移动高亮”和“变化任务提示”，但不再把“已手工移动”混同为“业务锁定”。
- 错误原因：
  - `IsLocked` 是业务锁定语义；
  - “刚刚手工移动过”只是前端展示语义；
  - 两者被错误混用，导致第二次拖拽被前端自己拒绝，而不是被真实排程规则拒绝。
- 验证计划：
  - 重启 `BlazorApp1`
  - 打开 `/gantt/syncfusion`
  - 输入 `PlanId=10096`
  - 勾选 `允许连带推移`
  - 连续对 `12122` 或 `12155` 执行两次拖拽
  - 确认：
    - 第一次拖拽成功后，第二次仍可继续进入拖拽编辑态；
    - 只有真正的锁定任务才会被前端拒绝
## 2026-03-19 - Syncfusion 时间轴缩小时过密修复（前端）

- 作用域：`frontend`
- 背景：
  - `/gantt/syncfusion` 在连续点击“缩小”后，顶部时间轴仍保留过细的底层日刻度；
  - 当时间范围扩展到多月时，月份标题下方会出现密集数字，导致时间轴难以辨认。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/codex_log.md`
- 处理：
  - 调整 `CustomZoomLevels` 的远距离缩放层级：
    - `Level 0` 从 `Month + Day(2天)` 改为 `Month + Week`
    - `Level 1` 从 `Day + Day` 改为 `Week + Day(2天)`
  - 同步把远距离缩放的时间标签改为更适合中文阅读的格式：
    - 月份显示为 `yyyy年M月`
    - 周起点显示为 `M月d日`
- 目的：
  - 在多月视图下保留时间结构感，但避免月份下方继续堆满按天数字；
  - 提高缩小时的时间轴可读性，不影响近距离的日/小时视图。
- 验证建议：
  - 打开 `/gantt/syncfusion`
  - 选择跨度 3 个月以上的计划
  - 连续点击“缩小”
  - 确认多月视图下顶部时间轴显示为“月份 + 周起点”，不再出现密集日数字

## 2026-03-19 - Syncfusion 适应窗口异常兜底修复（前端）

- 作用域：`frontend`
- 背景：
  - `/gantt/syncfusion` 点击顶部 `适应窗口` 按钮后，页面抛出未处理异常并提示刷新；
  - 异常路径集中在 `SfGantt.ZoomToFitAsync()`，而页面当前又启用了自定义时间轴缩放层级。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/codex_log.md`
- 处理：
  - 停止直接调用组件内部 `ZoomToFitAsync()`；
  - 改为页面自己的安全 fit 流程：
    - 先重算 `ChartStart / ChartEnd`
    - 再按需循环调用 `ZoomOutAsync()`
    - 配合 `hasHorizontalScroll()` 与 `notifyResize` 做稳定收口
  - 整个按钮事件外层加异常兜底，避免再次把页面打成未处理异常。
- 目的：
  - 优先恢复 `适应窗口` 按钮可用性；
  - 保留“尽量缩到刚好可看”的效果，同时避开组件内部不稳定入口。
- 验证建议：
  - 打开 `/gantt/syncfusion`
  - 点击 `适应窗口`
  - 确认页面不再出现未处理异常
  - 确认时间轴会回到合适范围，且仍能正常继续放大/缩小

## 2026-03-19 - 益模 MES v6.5 手册借鉴分析（前端/联动）

- 作用域：`both`
- 背景：
  - 读取参考文档 `D:\GF+\ASP_related\文档\益模制造执行系统v6 5操作通用.pdf`，评估其中哪些流程和能力对当前 APS/MES 项目有借鉴意义。
- 新增文档：
  - `docs/research/yimo-mes-v6_5-benchmark.md`
- 同步更新：
  - `docs/requirements/PRD.md`
- 结论摘要：
  - 当前系统最值得吸收的是“主计划 -> 工艺 -> 车间排程 -> 现场执行 -> 异常/外协/统计”的闭环思路，而不是旧式 UI。
  - 前端后续优先关注：
    - 异常单驱动返工 / 报废 / 重排
    - 任务执行闭环：开始 / 暂停 / 继续 / 完工
    - 资源负载与延期风险视图
    - 主计划与车间计划联动视图
- 说明：
  - 本次为参考分析与需求沉淀，不涉及代码逻辑变更。

## 2026-03-19 - Syncfusion 缩放链收敛、调试按钮与紧凑布局（前端/联调）

- 作用域：`frontend`
- 背景：
  - `/gantt/syncfusion` 在连续处理“放大 / 缩小 / 适应窗口”后出现明显卡顿；
  - `PlanId=10096` 在 `ProjectView` 下一度显示空图，需区分“前端渲染失败”还是“计划尚未排程”；
  - 页面联调时需要可复制的运行态诊断信息，且顶部工具区纵向占用偏大，甘特图主体下压明显。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `MES/BlazorApp1/BlazorApp1/wwwroot/js/gantt-asprova.js`
  - `MES/BlazorApp1/BlazorApp1/Services/GanttApiClient.cs`
  - `MES/BlazorApp1/BlazorApp1/Services/ApsApiService.cs`
  - `docs/codex_log.md`
- 处理：
  - 缩放链路收敛：
    - `ZoomIn / ZoomOut / ZoomToFit` 恢复为 Syncfusion 原生缩放主链；
    - 去掉缩放过程中的高频 `JS <-> .NET` 回调链；
    - 缩放完成后只保留一次前端轻量 `centerSelectedTaskIfNeeded(...)`，用于保持选中任务可见/近似居中。
  - 调试诊断：
    - 新增“调试信息”按钮与弹窗，支持复制当前 `PlanId / 视图 / 时间窗 / RV计数 / APS计数 / 渲染计数 / RV元数据 / APS任务样本`；
    - 删除页面常驻调试摘要行，避免正式界面长期占位。
  - 数据判定：
    - 用 `PlanId=10096` 联调确认：首次空图不是前端丢条，而是计划未排程；
    - 诊断值显示 `APS任务>0` 但 `APS已排=0` 时，甘特图无任务条属于数据状态正常表现；
    - 点击“自动排程”后，`APS已排 / RV任务 / 渲染任务` 同步转为非零，项目视图恢复出条。
  - 布局收紧：
    - 顶部卡片由 `p-3` 收紧为 `p-2`；
    - 输入区、状态提示、统计摘要和甘特图区之间的垂直间距统一下调；
    - 表单标签和输入控件做轻量紧凑化，甘特图主体整体上移。
- 目的：
  - 恢复缩放与适应窗口的流畅度，同时保留缩放后选中任务不易丢失的体验；
  - 把“空图”与“未排程”快速区分，避免继续误判前端渲染错误；
  - 在不破坏美观的前提下，减少顶部工具区对甘特主体的挤压。
- 验证建议：
  - 打开 `/gantt/syncfusion`
  - 输入 `PlanId=10096`
  - 先不自动排程，点击“调试信息”，确认可复制诊断文本且页面主体不出现常驻调试行；
  - 点击“自动排程”，确认 `ProjectView` 下任务条出现；
  - 选中任一任务后连续点击“放大 / 缩小 / 适应窗口”，确认缩放流畅且选中任务仍留在视口附近；
  - 观察顶部工具区与甘特图之间的距离，确认主体上移且界面未显得拥挤。
- 待实现：
  - 当前页面缩放共 5 档，分别为：
    - `Level 0`: `Month + Week`
    - `Level 1`: `Week + Day(Count=2)`
    - `Level 2`: `Day + Hour(Count=6)`
    - `Level 3`: `Day + Hour(Count=2)`
    - `Level 4`: `Day + Hour(Count=1)`
  - 后续需按 APS 使用场景重新评审这些档位是否足够均匀，尤其是远景可读性、中景节奏感和近景拖拽操作密度。
## 2026-03-20 - Syncfusion 适应窗口安全收口（前端）

- 作用域：`frontend`
- 背景：
  - `/gantt/syncfusion` 当前启用了自定义时间轴缩放层级与页面自管时间窗；
  - 代码里又重新直接调用了 `SfGantt.ZoomToFitAsync()`，这和仓库此前已经记录过的不稳定入口相冲突，存在再次触发异常或白板重绘问题的风险。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/playbook/lessons.md`
  - `docs/codex_log.md`
- 处理：
  - 停止在页面按钮中直接调用组件 `ZoomToFitAsync()`；
  - 改为安全 fit 流程：
    - 先按当前任务重算 `ChartStart / ChartEnd`
    - 再补一次 `notifyResize`
    - 若图表仍有横向滚动，则最多循环执行几次 `ZoomOutAsync()`
    - 最后继续保留“选中任务轻量保持可见”的前端收口
- 目的：
  - 保持 `适应窗口` 可用；
  - 同时尽量避开 Syncfusion 内部 `ZoomToFitAsync()` 在当前页面组合配置下的异常路径。

## 2026-03-20 - Syncfusion 保守版 5 档缩放方案落地（前端）

- 作用域：`frontend`
- 背景：
  - `/gantt/syncfusion` 之前的 5 档缩放仍偏向通用甘特视图：
    - `Level 1` 还是 `Week + Day(Count=2)`
    - `Level 3` 还是 `Day + Hour(Count=2)`
    - 最细档只有 `Day + Hour(Count=1)`
  - 结合当前 APS 场景，工序时长短则不足 1 小时，长则 4~5 小时；
  - 需要把“多个工序观察时精确到小时”和“最大放大时精确到分钟”同时纳入，但仍保持较稳妥的可读性与渲染密度。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/codex_log.md`
- 处理：
  - 将默认时间轴配置调整为与主工作档一致：
    - 默认视图改为 `Day + Hour(Count=6)`
    - 顶层标签改为更贴近中文阅读的 `M月d日`
  - 将页面自定义缩放 5 档改为保守版 APS 方案：
    - `Level 0`: `Month + Week`
    - `Level 1`: `Week + Day(Count=1)`
    - `Level 2`: `Day + Hour(Count=6)`
    - `Level 3`: `Day + Hour(Count=1)`
    - `Level 4`: `Hour + Minutes(Count=15)`
  - 其中最细档改为 Syncfusion 官方支持的 `Hour` 视图下分钟级底层刻度，用于短工序精查；
  - 没有继续激进到 `5 分钟 / 1 分钟`，先以 `15 分钟` 作为更稳妥的上限，避免时间轴过密和缩放跨度突变。
- 目的：
  - 让 `Level 2` 成为真正可长期停留的默认工作视图；
  - 让 `Level 3` 更适合同时观察多个短工序；
  - 让 `Level 4` 在不明显牺牲可读性的前提下，支持分钟级边界观察与拖拽微调。
- 验证建议：
  - 打开 `/gantt/syncfusion`
  - 选择同时包含短工序和 4~5 小时工序的 `PlanId`
  - 依次点击“放大 / 缩小”回归 5 档：
    - 确认 `Level 2` 下适合看多个工序的排程关系
    - 确认 `Level 3` 下可按小时精查多个短工序
    - 确认 `Level 4` 下底层时间轴变为 `15 分钟` 粒度，而不是仅到整点小时
  - 回归“适应窗口”，确认仍走页面既有安全收口，不因分钟级最细档重新触发异常路径

## 2026-03-20 - Syncfusion 时间轴格式串运行时异常修复（前端）

- 作用域：`frontend`
- 背景：
  - 在把 `/gantt/syncfusion` 的缩放档位调整为保守版 5 档后，页面运行时抛出：
    - `System.FormatException: The provided top or bottom tier format is invalid`
  - 触发点不是分钟级档位本身，而是 `TopTier / BottomTier.Format` 中使用了带中文字符的格式串，例如：
    - `yyyy年M月`
    - `M月d日`
    - `M月d日 HH:mm`
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/codex_log.md`
- 处理：
  - 保留分钟级最细档：
    - `Level 4 = Hour + Minutes(Count=15)`
  - 但将所有时间轴 `Format` 改回 Syncfusion 当前运行时可接受的安全格式：
    - `MMM yyyy`
    - `dd MMM`
    - `dd MMM yyyy`
    - `dd MMM HH:mm`
    - `HH:mm`
    - `mm`
- 结论：
  - 当前页面若继续使用 `CustomZoomLevels`，时间轴格式串不要直接写中文字符；
  - 若后续要恢复中文化时间轴，应优先评估是否改用组件支持的 formatter/template 路径，而不是继续把中文直接塞进 `Format`。
- 验证建议：
  - 强刷 `/gantt/syncfusion`
  - 连续点击“放大”直到最细档
  - 确认页面不再抛 `The provided top or bottom tier format is invalid`
  - 确认 `Level 4` 仍能显示 `15 分钟` 粒度，而不是回退到小时级

## 2026-03-20 - Syncfusion 首次拖动后第二次拖动失灵最小修复（前端）

- 作用域：`frontend`
- 背景：
  - 页面出现“第一次拖动成功后，第二次拖动不再生效”的现象；
  - 代码排查后，`IsApplyingTaskbarMove` 在 `finally` 中能正常复位，不像是简单状态锁死；
  - 最可疑链路出现在首次拖动成功且仅影响当前任务时的本地快路径：
    - 先局部更新任务
    - 再立即 `RefocusSelectedTaskAsync()`
    - 再立即 `RefreshSelectedTaskServerTruthAsync(...)`
  - 这组动作会在同一控件实例上二次修改当前任务对象，容易扰乱 Syncfusion 后续编辑态。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/codex_log.md`
- 处理：
  - 在 `OnTaskbarEdited` 成功且命中 `TryApplyLocalMoveResult(...)` 的分支里：
    - 保留本地任务更新时间与状态提示；
    - 去掉紧随其后的：
      - `Task.Delay(80)`
      - `RefocusSelectedTaskAsync()`
      - `RefreshSelectedTaskServerTruthAsync(...)`
  - 目的不是改变保存契约，而是避免一次成功拖动后立刻对同一任务再做异步聚焦和真值回写。
- 目的：
  - 尽量恢复任务条连续多次拖动能力；
  - 把改动限制在首次拖动成功后的前端快路径，不影响后端保存接口。
- 结果：
  - 用户已确认页面上“第一次拖动后第二次不能拖”的问题已解除；
  - 当前结论是：首次拖动成功后的即时 `RefocusSelectedTaskAsync + RefreshSelectedTaskServerTruthAsync` 确实会干扰后续拖拽编辑态，移除后连续拖动恢复正常。

## 2026-03-20 - Syncfusion 自动重排任务条颜色与图例不一致修复（前端）

- 作用域：`frontend`
- 背景：
  - 用户反馈自动排程/自动重排后的任务条仍显示为蓝色，与右上角图例中“手工移动”“自动重排变化”的橙色/绿色不一致；
  - 排查发现 `sync-taskbar-moved` / `sync-taskbar-rescheduled` 只挂在内层 `sync-taskbar-inner`；
  - 外层 Syncfusion 任务条壳 `e-gantt-child-taskbar` 仍保持默认蓝色视觉，导致最终显示与图例不一致。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `MES/BlazorApp1/BlazorApp1/wwwroot/css/gantt-syncfusion.css`
  - `docs/codex_log.md`
- 处理：
  - 在任务条模板新增 `BuildTaskbarShellClass(...)`，把 `sync-taskbar-moved` / `sync-taskbar-rescheduled` / `sync-taskbar-locked` 同时挂到外层 `sync-taskbar-shell`；
  - 在 `gantt-syncfusion.css` 中为外层 `e-gantt-child-taskbar.sync-taskbar-shell` 增加对应的 moved / rescheduled / moved+rescheduled 背景与边框样式；
  - 默认情况下显式清掉外层壳的透明度/阴影干扰，避免外层继续露出蓝色。
- 目的：
  - 让自动排程后的任务条主视觉颜色与图例一致；
  - 保持现有任务条模板、拖拽链和差异高亮逻辑不变，只修复颜色挂载层级错误。

## 2026-03-20 - 自动重排变化任务ID 改为排程完成后弹框（前端）

- 作用域：`frontend`
- 背景：
  - 原页面会在顶部甘特图上方额外占一行显示 `自动重排变化任务ID：[...]`；
  - 用户希望改成自动排程完成后弹出提示框，点击确认后消失，不再长期占位。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/codex_log.md`
- 处理：
  - 移除页面顶部对 `RescheduleDiffMessage` 的单独提示框渲染；
  - 在 `RunAutoScheduleAsync` 中保留差异统计逻辑，但在自动排程完成、重载数据、套用差异标记后，把这段消息改为通过现有 `UiDialog` 弹出；
  - 用户点击“知道了”后弹框消失，页面不再保留该提示占位。
- 目的：
  - 释放顶部垂直空间，让甘特图区保持更紧凑；
  - 保留自动排程后“哪些任务发生变化”的反馈，只改变呈现方式，不改排程逻辑。

## 2026-03-20 - 缩放后选中任务优先保持在视野内（前端）

- 作用域：`frontend`
- 背景：
  - 当前缩放后会调用 `centerSelectedTaskIfNeeded`；
  - 原逻辑在选中任务接近边缘或偏离中心较多时，容易直接把任务重新拉到中间，用户体感偏“跳”；
  - 用户要求是：缩放后任务条能留在缩放后的视野里，不需要再手动拉滚动条寻找，同时页面不要明显卡顿。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/wwwroot/js/gantt-asprova.js`
  - `docs/codex_log.md`
- 处理：
  - 把 `centerSelectedTaskIfNeeded` 的优先级改成：
    - 先判断选中任务是否已在可视区安全边距内；
    - 若只是部分出界或贴边，则只做“最小滚动量”的横向/纵向修正；
    - 只有最小修正后仍无法保证可见时，才退回原来的较强定位逻辑。
- 追加兜底：
  - 若轻量修正后任务条仍未进入视野，则调用现有 `refocusTask(...)` 做一次强制横向定位，避免出现“行已选中但任务条仍不在甘特图视野内”的情况。
- 进一步收口：
  - 用户实测仍存在“行高亮但任务条未回到视野”的场景；
  - 因此把 `ZoomIn / ZoomOut / ZoomToFit` 的收尾统一改为：缩放后先等待 `NotifyResizeStableAsync`，再固定调用一次 `refocusTask(...)`；
  - 目标是不再依赖轻量策略能否识别到任务条，而是保证缩放按钮链本身就把选中任务重新拉回可见区域。
- 根因补充：
  - 继续排查后发现这类失效更像是 Syncfusion 当前页面横向滚动不止一个容器；
  - 旧实现多数情况下只写单个 `content.scrollLeft/scrollTop`，可能导致左侧选中行已同步，但真正承载时间轴的容器没有一起滚动。
- 追加处理：
  - 新增 `getChartScrollers()` 与 `setChartScrollPosition(...)`；
  - 将 `scrollTimelineToStart / restoreScroll / refocusTask / centerSelectedTaskIfNeeded / notifyResizeDeep` 中涉及滚动的位置统一改为同步写所有候选图表滚动容器。
- 临时诊断：
  - 已把缩放定位诊断接入页面“调试信息”弹框；
  - 现在可直接查看：
    - 所有候选滚动容器的 `scrollLeft / scrollWidth / clientWidth`
    - `timelineStart / timelineEnd / pxPerDay`
    - 当前选中任务的 `TaskId / Start / End`
    - 当前是否存在 `.e-taskbar-selected` 对应的任务条 DOM 以及其屏幕位置
- 根据诊断得到的结论：
  - 在失败现场，`timelineStart=-`、`pxPerDay=0`、`selectedBar=no`；
  - 说明缩放后不能再依赖 `.e-taskbar-selected` 或时间轴度量来做横向定位。
- 追加修复：
  - 改为 `TaskId -> flatData rowIndex -> .e-chart-row[rowIndex] -> 行内任务条 DOM` 这条链；
  - `refocusTask(...)` 与 `centerSelectedTaskIfNeeded(...)` 现在优先按对应行直接找任务条，再按该任务条的 `offsetLeft / width` 做横向滚动；
  - 只有按行找条失败时，才继续退回原来的时间轴度量和 `scrollToDate(...)` 兜底。
- 最后兜底：
  - 继续实测后发现失败现场仍可能同时丢失“任务条 DOM”和“时间轴度量”；
  - 因此进一步把页面级 `ChartStart / ChartEnd` 传入 JS；
  - 新增按当前图表时间窗比例计算 `scrollLeft` 的兜底：即使 `timelineStart=-`、`pxPerDay=0`，也能按任务时间中点相对 `ChartStart / ChartEnd` 的比例把横向滚动拉到对应位置。
- 用户继续反馈：
  - 当前已能把“某些任务条”带进视野，但还不能稳定命中“当前选中任务条”。
- 继续收口：
  - `refocusTask(...)` 改成两段式：
    - 先纵向把目标行滚进可渲染区；
    - 等待一帧让虚拟化行和任务条真正渲染；
    - 再按该行中的任务条 DOM `offsetLeft / width` 做精确横向定位。
- 新增精确命中修复：
  - 实测发现前一版虽然能把“某个任务条”带进视野，但仍可能不是“当前选中任务条”；
  - 根因是 JS 仍在用“目标行中的第一个任务条 DOM”做横向定位，在虚拟化/行内 DOM 结构下可能命中错误元素；
  - 现已给前端任务条模板增加 `data-task-id`，并将 `refocusTask(...) / centerSelectedTaskIfNeeded(...)` 改为优先按 `TaskId` 精确查询 `.sync-taskbar-shell[data-task-id="..."]`；
  - 只有精确条形尚未渲染时，才退回行内查询与其他兜底路径。
- 再次诊断后的修复：
  - 用户现场诊断显示三个候选滚动容器横向位置已经发生变化，但 `scrollLeft` 分别落在 `16759 / 1809 / 3495`，未保持同一比例；
  - 说明“给所有候选容器写同一个绝对 `scrollLeft`”本身会把不同滚动宽度的容器写散，导致时间轴和任务层横向不同步；
- 现已将 `setChartScrollPosition(left, top)` 改为：按“最大可滚动宽度”计算目标比例，再把该比例映射到每个容器各自的可滚动范围；
- 目标是让不同宽度的图表容器保持横向同步，而不是继续把同一个像素值硬写给所有容器。
- 最新现场结论：
  - 调试信息已出现 `exactBar=yes`，说明当前任务条本体已经能被 `TaskId` 精确命中；
  - 剩余问题从“命中错误任务条”收缩为“缩放重绘过程中的偶发时序抖动”。
- 追加收口：
  - `refocusTask(...)` 现在不会在“刚写完滚动”后立刻返回；
  - 改为再等若干帧重新抓取 `exactBar`，并确认其真实进入可视区域后才判定定位成功；
  - 目标是减少“多数时候正确，但放大缩小多次后偶发一次没跟上”的情况。
- 目的：
  - 缩放后优先保证选中任务仍在视野里；
  - 减少每次缩放都强制居中的跳动感；
  - 保持 JS 侧轻量处理，不引入高频前后端联动。
- 最小交互收口：
  - 新增 `isAtMaxZoomLevel()`，按当前时间轴设置判断是否已处于最细档 `Level 4 = Hour + Minutes(15)`；
  - 若当前已经是最细档，再点击“放大”时只执行控件原生 `ZoomInAsync()`，不再触发后续选中任务重定位；
  - 目的：避免用户在最细档反复点击“放大”时出现无意义的二次定位和视图抖动。
- 进一步修正：
  - 用户反馈“最细档再点放大”时时间轴仍会动，说明运行时读取时间轴设置来判断最大档并不稳定；
  - 已改为前端页面自行维护 `CurrentZoomLevel`，默认档为 `Level 2`，`ZoomIn / ZoomOut / ZoomToFit` 都同步更新该值；
  - 现在只要 `CurrentZoomLevel >= 4`，点击“放大”会直接返回，真正做到“完全不做任何操作”。
- 缩放定位继续收口：
  - 用户现场诊断显示已能精确命中 `exactBar=yes`，但存在 `left=-2445,right=15,width=2460` 这类“只露出极窄边缘”的情况；
  - 根因不是没找到任务条，而是 JS 把“任务条只露出一点点”误判为已进入视野；
  - 现已收紧 `isTaskbarInViewport(...)` 的成功条件：除上下边距外，要求任务条可见宽度至少达到约 60%，且任务条中心点落入当前视口；
  - 目的：避免“仅剩 10~20px 挂在屏幕边缘”时停止继续校正，导致用户体感上仍像“定位不到任务条”。
- 缩小时继续发现：
  - 用户现场诊断出现 `exactBar=yes,left=11229,right=12800`，但 `e-chart-scroll-container.left=8126`、`e-chart-rows-container.left=1`；
  - 说明缩放后的官方 API 主要滚动了主时间轴容器，而承载任务条的 `e-chart-rows-container` 没有同步横向位置；
  - 现已新增 `syncHorizontalScrollersFromPrimary(...)`，在官方 API 定位后和微调滚动后，都用主容器当前比例回写其他横向容器；
  - 目的：避免“主时间轴已滚到目标附近，但任务条层仍停在旧位置”，特别是缩小时再次出现“时间轴在目标附近、任务条仍不在视野里”的现象。
- 再次缩小时现场诊断：
  - 用户继续提供 `exactBar=yes,left=-3013,right=-553` 的失败现场，说明任务条这次不是停在右侧未跟上，而是被横向微调推过头，整个跑到视口左侧；
  - 结合 `e-chart-scroll-container.left=25619`、`e-chart-rows-container.left=7528` 判断，问题更接近“把同一个 `deltaX` 同时写入多个宽度不同的横向容器”导致主容器和任务条层被一起推歪；
  - 现已将 `nudgeHorizontalScrollersByDelta(...)` 改为只修改主图表容器，再按主容器当前滚动比例同步其它横向容器；
  - 目的：避免缩小时的横向微调把主时间轴 overscroll 到错误位置，导致任务条整体跑到视口左侧。
- 用户要求接受“慢一点但更稳”，因此缩放链已切到更重但更稳的执行方式：
  - 页面新增 `IsZooming` 状态，在甘特图区内显示轻量圆圈 loading；
  - `放大 / 缩小 / 适应窗口` 在执行期间串行化，按钮临时禁用，不再接受连续重入；
  - 每次缩放后先等待控件布局稳定（`NotifyResizeStableAsync + 延时`），再统一执行选中任务定位；
  - 目标是放弃高时序敏感的“缩放后立刻抢定位”，以更慢但更稳的方式减少缩放后偶发丢失任务条的问题，同时保持鼠标可移动、不冻结整个页面。
- 用户继续提供失败现场截图：
  - 诊断显示 `exactBar=yes,left=5,right=35,width=31`，但截图中的真实图表区左边界远不在该位置；
  - 这说明当前 `TaskId -> DOM` 命中并非总是拿到“当前图表区内的真实任务条”，而可能拿到虚拟化/重绘过程中遗留的同 `TaskId` 其它节点；
  - 现已把 `getTaskbarElementByTaskId(...)` 从单纯 `querySelector` 改为 `querySelectorAll + 候选评分`：优先选择最接近当前图表可视区中心、与当前主图表容器最接近的那一个任务条节点；
  - 目的：避免在同一 `TaskId` 存在多份 DOM 候选时，误用错误节点做定位，导致诊断显示“已找到任务条”，但实际用户看到的并不是当前视图中的那一根。
- 继续按“根治”方向调整：
  - 用户确认接受“慢一点但更稳”，并提出直接保留任务条时间轴坐标的思路；
  - 现已将缩放后的横向定位主链改为“时间坐标驱动”：官方 API 只保留 `SelectRowAsync + ScrollIntoViewAsync` 处理纵向选中与滚到对应行，不再依赖 `ScrollToTaskbarAsync` 做横向定位；
  - JS 新增 `focusTaskByTimeCoordinates(taskId, startAt, endAt, chartStart, chartEnd)`，直接根据任务 `Start/End` 和当前 `ChartStart/ChartEnd` 计算任务中点应落到的 `scrollLeft`，再同步其它横向容器；
  - DOM 任务条查询只保留在最后一步做轻量校正和诊断，不再担任横向主定位真值来源；
  - 目标：让横向定位主要依赖任务时间真值，而不是依赖虚拟化和重绘下不稳定的任务条 DOM。
- 缩放档位状态继续收口：
  - 用户现场发现改了时间窗后，视觉上还没到最细档，但“放大”按钮已经不再生效；
  - 根因是页面自维护的 `CurrentZoomLevel` 只在按钮缩放链里递增/递减，未在 `SetChartRange(...)` 这种“时间窗/数据重载导致图表范围重算”的路径中复位；
  - 现已在 `SetChartRange(...)` 开始时统一把 `CurrentZoomLevel` 复位为 `DefaultZoomLevel`；
  - 目的：避免改时间窗、刷新、自动排程重载后，视觉档位与页面内记录的缩放档位脱节，导致“看起来还能放大，但按钮误判已到最大档”。
- 再次现场反馈表明：
  - 仅在 `SetChartRange(...)` 重置 `CurrentZoomLevel` 还不够，因为控件有时会忽略一次 `ZoomInAsync()` / `ZoomOutAsync()`，但页面仍然按成功处理去做本地 `+1 / -1`，最终继续把缩放档位记漂；
  - 现已新增 `ganttAsprova.getCurrentZoomLevel()`，直接按 Syncfusion 当前时间轴设置映射到 0..4 五档；
  - Razor 侧 `ZoomIn / ZoomOut / ZoomToFit / ExecuteZoomWorkflowAsync` 都改成在缩放前后向控件回读真实档位，不再盲目本地递增或递减；
  - 目标：让“是否还能继续放大/缩小”的判断以控件当前真实档位为准，避免按钮因为本地状态漂移而提前失效。
- 时间坐标主链继续收口：
  - 用户继续提供 `exactBar=yes,left=1633,right=1789` 这类现场，说明当前主视口仍被带到错误横向位置，任务条层还停在右侧；
  - 进一步判断后发现：横向主滚动不该优先写给时间轴层，而应优先写给真正承载任务条的 `e-chart-rows-container`；
  - 现已新增 `getHorizontalPrimaryScroller()`，横向主容器优先选择 `e-chart-rows-container`，其次才是 `e-chart-scroll-container`；
  - `setChartScrollPosition(left, top)` 也已改为：先把 `left` 写入横向主容器，再按主容器当前位置同步其它横向层，而不是继续按“最大宽度比例”给所有层同时分发；
  - 目标：让“按时间坐标算出来的横向位置”先落到任务条实际承载层，再让时间轴头部去跟随，而不是反过来。
- 2026-03-20 继续现场回归后确认：上述“任务条层优先为横向主容器”的判断不成立。
  - 用户继续提供 `exactBar=yes,left=-13158,right=-10698` 这类现场，同时 `e-chart-scroll-container.sw=22118`、`e-chart-rows-container.sw=7061`；
  - 说明 `e-chart-rows-container` 与主时间轴容器并不共享同一横向滚动坐标系，把它提升为主容器会直接把时间坐标定位带偏；
  - 现已回退 `getHorizontalPrimaryScroller()` 的优先级：恢复以 `e-chart-scroll-container` 为横向主容器，`e-chart-rows-container` 只做跟随同步；
  - 结论：时间坐标驱动仍应落在主时间轴滚动容器上，任务条层不能作为横向真值源。
- 2026-03-20 当日总结：
  - 已完成：`/gantt/syncfusion` 缩放档位调整为 5 档，最细档为 `Hour + Minutes(15)`；顶部布局收紧；摘要提示右移；“自动重排变化任务ID”改为弹框；自动排程后的任务条颜色已与图例对齐。
  - 已完成：拖拽链最小修复，移除首次拖动成功后的即时 `RefocusSelectedTaskAsync + RefreshSelectedTaskServerTruthAsync`，用户已确认“第一次拖动后第二次不能拖”的问题解除。
  - 已完成：缩放按钮链已改为串行化并显示轻量 loading，最细档再点“放大”走 no-op；缩放档位状态改为从控件真实时间轴设置回读，不再盲目本地递增/递减。
  - 已完成：缩放后选中任务定位主链改为“纵向走官方 API、横向走时间坐标”，DOM 任务条只保留轻量校正和诊断；同时确认 `e-chart-rows-container` 不能作为横向主容器，当前已回退为 `e-chart-scroll-container` 主导。
  - 已完成：后端中文工序名问题已在另一仓库收口，包括 UTF-8 安全导入、坏数据恢复、显示兜底和历史乱码字面量清理；前端任务条已不再显示 `???`。
  - 当前状态：缩放后选中任务定位相比最差阶段已有明显改善，但仍未完全稳定；用户 2026-03-20 最后一次现场反馈表明，当前版本仍可能出现“左侧已选中、右侧任务条未进入视野”的场景，后续应继续沿“主时间轴容器为横向真值”这条线收口，不再回到任务条层主导方案。

## 2026-03-23 - Syncfusion 最细缩放档年份显示异常修复（前端）

- 作用域：`frontend`
- 背景：
  - 用户在 `/gantt/syncfusion` 选中 `TaskId=22122` 时反馈：任务真实开始时间为 `2026-03-19 12:30`，但最细缩放档上方时间轴却出现了 `2025`；
  - 联调核对后端真值：
    - `GET /api/plans/10096/tasks` 中 `TaskId=22122.plannedStart = 2026-03-19T12:30:00`
    - `GET /api/gantt/syncfusion/resource-view?planId=10096` 中 `meta.start = 2026-03-18T00:00:00`
  - 结论：不是后端返回成了 2025，而是前端最细时间轴显示与缩放档位识别存在问题。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `MES/BlazorApp1/BlazorApp1/wwwroot/js/gantt-asprova.js`
  - `docs/codex_log.md`
- 处理：
  - 将最细缩放档从 `Hour + Minutes(15)` 调整为更稳定的 `Day + Minutes(15)`：
    - `TopTier: Day / dd MMM yyyy`
    - `BottomTier: Minutes / HH:mm / Count=15`
    - `TimelineViewMode: Day`
  - 修正 `gantt-asprova.js` 中 `isAtMaxZoomLevel()` 与 `getCurrentZoomLevel()` 的档位识别常量，使其与 Razor 中实际定义一致：
    - Level 1 `TimelineUnitSize` 改回 `56`
    - Level 3 `TimelineUnitSize` 改回 `92`
    - Level 4 改为识别 `Day + Minutes(15)`
- 目的：
  - 避免最细档在头部时间轴上混出错误年份；
  - 同时消除“控件实际缩放档位”和前端 JS 自己识别的档位不一致”带来的边界问题。
- 验证：
  - 后端真值核对：
    - `PlanId=10096`
    - `TaskId=22122`
    - `plannedStart = 2026-03-19 12:30:00`
    - `resource-view meta.start = 2026-03-18 00:00:00`
  - 前端回归建议：
    - 打开 `/gantt/syncfusion`
    - 输入 `PlanId=10096`
    - 选中 `TaskId=22122`
    - 连续点击“放大”到最细档
    - 确认顶部时间轴显示为 `2026`，且底部 15 分钟刻度仍正常

## 2026-03-23 - Syncfusion 最细缩放档刻度过密修复（前端）

- 作用域：`frontend`
- 背景：
  - 用户在修复年份异常后继续反馈：`/gantt/syncfusion` 放大到最细档时，底部时间轴在每个 `15 分钟` 单元格都显示完整 `HH:mm`，导致出现一整排密密麻麻的时间；
  - 本地包文档虽然能看到 `GanttTimelineTierSettings.Formatter / FormatterTemplate` 说明，但在当前项目实际运行时，`BottomTier.Formatter` 会触发 `Microsoft.CSharp.RuntimeBinder.RuntimeBinderException`；
  - 结论：当前版本先不要把 formatter 挂到最细档时间轴，优先回到不触发运行时异常的稳定格式组合。
- 改了哪些文件：
  - `MES/BlazorApp1/BlazorApp1/Pages/GanttSyncfusion.razor`
  - `docs/codex_log.md`
- 处理：
  - 保留最细档时间粒度与时间轴层级不变：
    - `TopTier: Day / dd MMM yyyy`
    - `BottomTier: Minutes / Count=15`
  - 移除会触发运行时异常的 formatter 尝试；
  - 将最细档底部时间轴文案从 `HH:mm` 收窄为 `mm`，让同样的 `15 分钟` 网格只显示 `00 / 15 / 30 / 45`。
- 目的：
  - 保持 `15 分钟` 观察粒度，方便短工序边界检查；
  - 避免最细档时间轴出现整排重复完整时间文本，同时避开当前 Syncfusion 版本的 formatter 运行时异常。
- 验证建议：
  - 打开 `/gantt/syncfusion`
  - 输入 `PlanId=10096`
  - 连续点击“放大”到最细档
  - 确认页面不再抛 `RuntimeBinderException`
  - 确认底部时间轴显示为 `00 / 15 / 30 / 45`
  - 确认 `15 分钟` 网格仍存在，任务条位置不受影响
