window.ApsGantt = (function () {
    var schedulePicker;
    var markerId = null;
    function applyScale(viewMode) {
        if (viewMode === 'Month') {
            gantt.config.scale_unit = 'month';
            gantt.config.date_scale = '%Y-%m';
            gantt.config.subscales = [{ unit: 'week', step: 1, date: 'W%W' }];
        } else if (viewMode === 'Week') {
            gantt.config.scale_unit = 'week';
            gantt.config.date_scale = 'W%W';
            gantt.config.subscales = [{ unit: 'day', step: 1, date: '%m-%d' }];
        } else {
            gantt.config.scale_unit = 'day';
            gantt.config.date_scale = '%m-%d';
            gantt.config.subscales = [{ unit: 'hour', step: 2, date: '%H:%i' }];
        }
    }

    function render(elementId, data, dotNetRef, options) {
        var container = document.getElementById(elementId);
        if (!container) {
            return;
        }

        if (!data || data.length === 0) {
            container.innerHTML = '<div class="text-muted p-3">暂无任务</div>';
            return;
        }

        container.innerHTML = '';
        applyScale(options && options.view_mode ? options.view_mode : 'Day');

        gantt.config.date_format = '%Y-%m-%d %H:%i';
        gantt.config.readonly = true;
        gantt.config.drag_move = false;
        gantt.config.drag_progress = false;
        gantt.config.drag_resize = false;
        gantt.config.show_progress = false;
        gantt.config.autosize = 'y';
        gantt.config.grid_width = 300;
        gantt.config.row_height = 42;
        gantt.config.task_height = 26;
        gantt.config.show_links = true;
        gantt.config.drag_links = false;

        gantt.templates.task_class = function (start, end, task) {
            if (task.locked) {
                return 'locked';
            }
            if (task.urgent) {
                return 'urgent';
            }
            return '';
        };

        gantt.attachEvent('onAfterTaskDrag', function (id, mode) {
            if (!dotNetRef) {
                return true;
            }
            if (mode === gantt.config.drag_mode.move || mode === gantt.config.drag_mode.resize) {
                var task = gantt.getTask(id);
                const taskId = parseInt(task.id, 10);
                if (Number.isFinite(taskId)) {
                    dotNetRef.invokeMethodAsync('OnGanttDateChange', taskId, task.start_date.toISOString(), task.end_date.toISOString());
                }
            }
            return true;
        });

        gantt.attachEvent('onTaskClick', function (id) {
            if (!dotNetRef) {
                return true;
            }
            const taskId = parseInt(id, 10);
            if (Number.isFinite(taskId)) {
                dotNetRef.invokeMethodAsync('OnGanttClick', taskId);
            }
            return true;
        });

        gantt.init(elementId);
        gantt.clearAll();
        if (typeof gantt.addMarker === 'function') {
            if (markerId) {
                gantt.deleteMarker(markerId);
                markerId = null;
            }
            markerId = gantt.addMarker({
                start_date: new Date(),
                css: 'gantt_marker_today',
                text: ''
            });
        }
        var links = options && options.links ? options.links : [];
        gantt.parse({ data: data, links: links });
    }

    function initScheduleRange(rangeInputId, startInputId, endInputId) {
        var rangeInput = document.getElementById(rangeInputId);
        if (!rangeInput || typeof flatpickr === 'undefined') {
            return;
        }

        schedulePicker = flatpickr(rangeInput, {
            mode: 'range',
            enableTime: true,
            time_24hr: true,
            dateFormat: 'Y-m-d H:i',
            onChange: function (selectedDates) {
                if (!selectedDates || selectedDates.length < 2) {
                    return;
                }
                var start = selectedDates[0];
                var end = selectedDates[1];
                var startInput = document.getElementById(startInputId);
                var endInput = document.getElementById(endInputId);
                if (startInput && endInput) {
                    startInput.value = formatDate(start);
                    endInput.value = formatDate(end);
                    startInput.dispatchEvent(new Event('input'));
                    endInput.dispatchEvent(new Event('input'));
                }
            }
        });
    }

    function setScheduleRange(startValue, endValue) {
        if (!schedulePicker) {
            return;
        }
        if (!startValue || !endValue) {
            return;
        }
        schedulePicker.setDate([startValue, endValue], true, 'Y-m-d H:i');
    }

    function ensureResourceGanttLoaded() {
        if (window.ApsResourceGantt && typeof window.ApsResourceGantt.registerDotNet === 'function') {
            return Promise.resolve(true);
        }

        return new Promise(function (resolve) {
            var existing = document.querySelector('script[src*="aps-resource-gantt.js"]');
            if (existing && existing.dataset.apsGanttLoaded === '1') {
                resolve(!!(window.ApsResourceGantt && typeof window.ApsResourceGantt.registerDotNet === 'function'));
                return;
            }

            var script = existing || document.createElement('script');
            if (!script.src) {
                script.src = '/aps-gantt/aps-resource-gantt.js';
            }
            script.async = true;
            script.onload = function () {
                script.dataset.apsGanttLoaded = '1';
                resolve(!!(window.ApsResourceGantt && typeof window.ApsResourceGantt.registerDotNet === 'function'));
            };
            script.onerror = function () {
                resolve(false);
            };

            if (!existing) {
                document.head.appendChild(script);
            }
        });
    }

    function formatDate(date) {
        var yyyy = date.getFullYear();
        var mm = String(date.getMonth() + 1).padStart(2, '0');
        var dd = String(date.getDate()).padStart(2, '0');
        var hh = String(date.getHours()).padStart(2, '0');
        var mi = String(date.getMinutes()).padStart(2, '0');
        return yyyy + '-' + mm + '-' + dd + ' ' + hh + ':' + mi;
    }

    return {
        render: render,
        initScheduleRange: initScheduleRange,
        setScheduleRange: setScheduleRange,
        ensureResourceGanttLoaded: ensureResourceGanttLoaded
    };
})();
