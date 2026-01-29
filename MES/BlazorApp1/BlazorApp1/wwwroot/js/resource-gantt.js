window.ResourceGanttInterop = (function () {
    var syncMap = new Map();

    function initScrollSync(leftId, rightId, headerId) {
        var left = document.getElementById(leftId);
        var right = document.getElementById(rightId);
        var header = document.getElementById(headerId);
        if (!left || !right) {
            return;
        }

        var key = leftId + "|" + rightId + "|" + headerId;
        if (syncMap.has(key)) {
            return;
        }

        var isSyncing = false;
        function syncFromLeft() {
            if (isSyncing) return;
            isSyncing = true;
            right.scrollTop = left.scrollTop;
            isSyncing = false;
        }

        function syncFromRight() {
            if (isSyncing) return;
            isSyncing = true;
            left.scrollTop = right.scrollTop;
            if (header) {
                header.scrollLeft = right.scrollLeft;
            }
            isSyncing = false;
        }

        left.addEventListener("scroll", syncFromLeft, { passive: true });
        right.addEventListener("scroll", syncFromRight, { passive: true });

        if (header) {
            header.addEventListener("scroll", function () {
                if (isSyncing) return;
                isSyncing = true;
                right.scrollLeft = header.scrollLeft;
                isSyncing = false;
            }, { passive: true });
        }

        syncMap.set(key, { left: left, right: right, header: header });
    }

    function getScrollInfo(elementId) {
        var el = document.getElementById(elementId);
        if (!el) return null;
        return {
            scrollTop: el.scrollTop,
            scrollLeft: el.scrollLeft,
            clientWidth: el.clientWidth,
            clientHeight: el.clientHeight
        };
    }

    function setScrollLeft(elementId, value) {
        var el = document.getElementById(elementId);
        if (!el) return;
        el.scrollLeft = value;
    }

    function getBarRects(containerId, barSelector) {
        var container = document.getElementById(containerId);
        if (!container) {
            return [];
        }
        var cRect = container.getBoundingClientRect();
        var bars = container.querySelectorAll(barSelector);
        var result = [];
        bars.forEach(function (bar) {
            var rect = bar.getBoundingClientRect();
            var id = bar.getAttribute("data-task-id");
            if (!id) return;
            result.push({
                id: id,
                x: rect.left - cRect.left + container.scrollLeft,
                y: rect.top - cRect.top + container.scrollTop,
                width: rect.width,
                height: rect.height
            });
        });
        return result;
    }

    function logSelectedBar(containerId, taskId, pxPerMinute) {
        var container = document.getElementById(containerId);
        if (!container) {
            console.warn("ResourceGanttInterop: container not found", containerId);
            return;
        }
        var selector = '[data-task-id="' + taskId + '"]';
        var bar = container.querySelector(selector);
        if (!bar) {
            console.warn("ResourceGanttInterop: bar not found", taskId);
            return;
        }
        var rect = bar.getBoundingClientRect();
        var rowIndex = bar.getAttribute("data-row-index");
        console.log("[ResourceGantt] selected bar", {
            taskId: taskId,
            rowIndex: rowIndex,
            rect: rect,
            scrollLeft: container.scrollLeft,
            pxPerMinute: pxPerMinute
        });
    }

    return {
        initScrollSync: initScrollSync,
        getScrollInfo: getScrollInfo,
        setScrollLeft: setScrollLeft,
        getBarRects: getBarRects,
        logSelectedBar: logSelectedBar
    };
})();
