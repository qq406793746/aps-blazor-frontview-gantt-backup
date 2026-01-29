## 2026-01-26 17:00

- Ŀ�꣺��̬�Ų�����ͼ��Ϊ�������ɫ��ֻ��չʾ��ƥ��Ŀ����ʽ����¼�����־��
- �޸�ʱ�䣺2026-01-26 17:00
- ������Щ�ļ���
  - MES\BlazorApp1\BlazorApp1\Pages\DynamicScheduling.razor
  - MES\BlazorApp1\BlazorApp1\wwwroot\js\aps-gantt.js
  - MES\BlazorApp1\BlazorApp1\wwwroot\aps-gantt\aps-resource-gantt.js
- �����֤��
  - ��� BlazorApp1�����롰��̬�Ų���ҳ��
  - ��������ظ��ء���ȷ����������ɫ����������Ҳ�����ק/����
  - ��Դ��������������ɫһ�¡�������ʾ����
- δ������
  - δʵ��������������ʾ/��ʽ��һ������Ŀ��ͼ2
  - δ�ṩ��ϸ����ɫ����/ͼ��

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
