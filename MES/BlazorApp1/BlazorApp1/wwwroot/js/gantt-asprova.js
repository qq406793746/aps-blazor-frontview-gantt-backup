window.ganttAsprova = window.ganttAsprova || {};

window.ganttAsprova.getMetrics = function () {
    const content = document.querySelector('.e-gantt-content') || document.querySelector('.e-chart-scroll-container');
    const timeline = document.querySelector('.e-timeline-header-container');
    const chartRow = document.querySelector('.e-chart-row') || document.querySelector('.e-row');
    const chart = document.querySelector('.e-gantt-chart');

    const scrollLeft = content ? content.scrollLeft : 0;
    const scrollTop = content ? content.scrollTop : 0;
    const timelineWidth = timeline ? timeline.getBoundingClientRect().width : 0;
    const chartHeight = chart ? chart.getBoundingClientRect().height : 0;
    const rowHeight = chartRow ? chartRow.getBoundingClientRect().height : 0;
    const pxPerHour = timelineWidth > 0 ? timelineWidth / (24 * 14) : 0;

    return {
        scrollLeft,
        scrollTop,
        timelineWidth,
        chartHeight,
        rowHeight,
        pxPerHour
    };
};
