"use strict";
/**
 * AppBlueprint UiKit - Site-wide JavaScript interop module
 * Provides comprehensive Blazor interoperability for UI components
 */
// ========================================================================
// THEME MANAGEMENT
// ========================================================================
const themeManager = {
    toggleDarkMode() {
        const html = document.documentElement;
        if (html.classList.contains('dark')) {
            html.classList.remove('dark');
            html.style.colorScheme = 'light';
            localStorage.setItem('theme', 'light');
        }
        else {
            html.classList.add('dark');
            html.style.colorScheme = 'dark';
            localStorage.setItem('theme', 'dark');
        }
    },
    isDarkMode() {
        return document.documentElement.classList.contains('dark');
    },
    getTheme() {
        return localStorage.getItem('theme') || 'system';
    },
    setTheme(theme) {
        const html = document.documentElement;
        localStorage.setItem('theme', theme);
        if (theme === 'dark') {
            html.classList.add('dark');
            html.style.colorScheme = 'dark';
        }
        else if (theme === 'light') {
            html.classList.remove('dark');
            html.style.colorScheme = 'light';
        }
        else if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
            html.classList.add('dark');
            html.style.colorScheme = 'dark';
        }
        else {
            html.classList.remove('dark');
            html.style.colorScheme = 'light';
        }
    }
};
// ========================================================================
// ELEMENT POSITION UTILITIES
// ========================================================================
function getElementRect(selector) {
    const element = document.querySelector(selector);
    if (element === null) {
        return null;
    }
    const rect = element.getBoundingClientRect();
    return {
        top: rect.top + window.scrollY,
        left: rect.left + window.scrollX,
        right: rect.right + window.scrollX,
        bottom: rect.bottom + window.scrollY,
        width: rect.width,
        height: rect.height
    };
}
// ========================================================================
// SIDEBAR MANAGEMENT
// ========================================================================
const sidebarManager = {
    toggleExpanded() {
        console.log('sidebarManager.toggleExpanded called');
        const body = document.querySelector('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        const isExpanded = body.classList.contains('sidebar-expanded');
        if (isExpanded) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
            console.log('Sidebar collapsed');
        }
        else {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
            console.log('Sidebar expanded');
        }
    },
    expand() {
        console.log('sidebarManager.expand called');
        const body = document.querySelector('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        if (!body.classList.contains('sidebar-expanded')) {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
            console.log('Sidebar expanded (was collapsed)');
        }
        else {
            console.log('Sidebar already expanded');
        }
    },
    collapse() {
        console.log('sidebarManager.collapse called');
        const body = document.querySelector('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        if (body.classList.contains('sidebar-expanded')) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
            console.log('Sidebar collapsed (was expanded)');
        }
        else {
            console.log('Sidebar already collapsed');
        }
    },
    isExpanded() {
        const body = document.querySelector('body');
        if (body === null) {
            console.error('Body element not found');
            return false;
        }
        const expanded = body.classList.contains('sidebar-expanded');
        console.log('sidebarManager.isExpanded:', expanded);
        return expanded;
    }
};
console.log('sidebarManager initialized:', typeof sidebarManager);
// ========================================================================
// CSS CUSTOM PROPERTY HELPERS
// ========================================================================
const cssHelper = {
    getCssVariable(variable) {
        return getComputedStyle(document.documentElement).getPropertyValue(variable).trim();
    },
    adjustColorOpacity(color, opacity) {
        if (color.startsWith('#')) {
            return this.adjustHexOpacity(color, opacity);
        }
        else if (color.startsWith('hsl')) {
            return this.adjustHSLOpacity(color, opacity);
        }
        else if (color.startsWith('oklch')) {
            return this.adjustOKLCHOpacity(color, opacity);
        }
        return color;
    },
    adjustHexOpacity(hex, opacity) {
        const r = parseInt(hex.slice(1, 3), 16);
        const g = parseInt(hex.slice(3, 5), 16);
        const b = parseInt(hex.slice(5, 7), 16);
        return `rgba(${r}, ${g}, ${b}, ${opacity})`;
    },
    adjustHSLOpacity(hsl, opacity) {
        return hsl.replace(')', `, ${opacity})`).replace('hsl', 'hsla');
    },
    adjustOKLCHOpacity(oklch, opacity) {
        return oklch.replace(')', ` / ${opacity})`);
    }
};
// ========================================================================
// CLICK OUTSIDE HANDLER
// ========================================================================
const clickOutsideHelper = {
    register(element, dotNetHelper, methodName) {
        const handler = (event) => {
            if (element !== null && !element.contains(event.target)) {
                dotNetHelper.invokeMethodAsync(methodName);
            }
        };
        document.addEventListener('click', handler);
        return handler;
    },
    unregister(handler) {
        document.removeEventListener('click', handler);
    }
};
// ========================================================================
// DROPDOWN MANAGEMENT
// ========================================================================
const dropdownManager = {
    handlers: new Map(),
    initialize(dropdown, trigger, dotNetHelper) {
        const clickHandler = (event) => {
            if (!dropdown.contains(event.target) && !trigger.contains(event.target)) {
                dotNetHelper.invokeMethodAsync('CloseFromJS');
            }
        };
        const keyHandler = (event) => {
            if (event.keyCode === 27) { // ESC key
                dotNetHelper.invokeMethodAsync('CloseFromJS');
            }
        };
        document.addEventListener('click', clickHandler);
        document.addEventListener('keydown', keyHandler);
        this.handlers.set(dropdown, { clickHandler, keyHandler });
    },
    cleanup(dropdown) {
        const handlers = this.handlers.get(dropdown);
        if (handlers) {
            document.removeEventListener('click', handlers.clickHandler);
            document.removeEventListener('keydown', handlers.keyHandler);
            this.handlers.delete(dropdown);
        }
    }
};
// ========================================================================
// MODAL MANAGEMENT
// ========================================================================
const modalManager = {
    handlers: new Map(),
    _registerEventHandlers(targetElement, dotNetHelper, includeTargetInCloseCheck) {
        const clickHandler = (event) => {
            if (includeTargetInCloseCheck && !targetElement.contains(event.target)) {
                dotNetHelper.invokeMethodAsync('CloseFromJS');
            }
            else if (!includeTargetInCloseCheck) {
                // For cases where closing should happen regardless of target (e.g., global ESC)
                dotNetHelper.invokeMethodAsync('CloseFromJS');
            }
        };
        const keyHandler = (event) => {
            if (event.keyCode === 27) { // ESC key
                dotNetHelper.invokeMethodAsync('CloseFromJS');
            }
        };
        document.addEventListener('click', clickHandler);
        document.addEventListener('keydown', keyHandler);
        return { clickHandler, keyHandler };
    },
    initialize(modalContent, dotNetHelper) {
        const { clickHandler, keyHandler } = this._registerEventHandlers(modalContent, dotNetHelper, true);
        this.handlers.set(modalContent, { clickHandler, keyHandler });
    },
    initializeSearch(modalContent, searchInput, dotNetHelper) {
        const { clickHandler, keyHandler } = this._registerEventHandlers(modalContent, dotNetHelper, true);
        this.handlers.set(modalContent, { clickHandler, keyHandler });
        // Focus search input
        if (searchInput !== null) {
            setTimeout(() => searchInput.focus(), 100);
        }
    },
    cleanup(modalContent) {
        const handlers = this.handlers.get(modalContent);
        if (handlers) {
            document.removeEventListener('click', handlers.clickHandler);
            document.removeEventListener('keydown', handlers.keyHandler);
            this.handlers.delete(modalContent);
        }
    }
};
// ========================================================================
// DATEPICKER MANAGEMENT
// ========================================================================
const datepickerManager = {
    instances: {},
    initialize(input, align) {
        if (typeof window.flatpickr === 'undefined') {
            console.error('Flatpickr library not loaded');
            return;
        }
        // Destroy existing instance if any
        if (this.instances[input.id] !== null) {
            this.instances[input.id].destroy();
        }
        const customClass = align || '';
        // Initialize flatpickr with same config as Vue template
        this.instances[input.id] = window.flatpickr(input, {
            mode: 'range',
            static: true,
            monthSelectorType: 'static',
            dateFormat: 'M j, Y',
            defaultDate: [new Date(new Date().setDate(new Date().getDate() - 6)), new Date()],
            prevArrow: '<svg class="fill-current" width="7" height="11" viewBox="0 0 7 11"><path d="M5.4 10.8l1.4-1.4-4-4 4-4L5.4 0 0 5.4z" /></svg>',
            nextArrow: '<svg class="fill-current" width="7" height="11" viewBox="0 0 7 11"><path d="M1.4 10.8L0 9.4l4-4-4-4L1.4 0l5.4 5.4z" /></svg>',
            onReady(selectedDates, dateStr, instance) {
                instance.element.value = dateStr.replace('to', '-');
                if (customClass) {
                    instance.calendarContainer.classList.add(`flatpickr-${customClass}`);
                }
            },
            onChange(selectedDates, dateStr, instance) {
                instance.element.value = dateStr.replace('to', '-'); // Corrected typo here (Freplace -> replace)
            }
        });
    }
};
// ========================================================================
// COMMAND PALETTE MANAGEMENT
// ========================================================================
const commandPaletteManager = {
    dotNetHelper: null,
    initialize(dotNetHelper) {
        this.dotNetHelper = dotNetHelper;
        // Listen for Ctrl+K keyboard shortcut
        document.addEventListener('keydown', (event) => {
            if ((event.ctrlKey || event.metaKey) && event.key === 'k') {
                event.preventDefault();
                if (this.dotNetHelper !== null) {
                    dotNetHelper.invokeMethodAsync('OpenFromJS');
                }
            }
        });
    }
};
// ========================================================================
// INFINITE SCROLL MANAGEMENT
// ========================================================================
const infiniteScrollManager = {
    observers: new Map(),
    initialize(sentinelElement, dotNetHelper, threshold) {
        if (sentinelElement === null) {
            return;
        }
        const options = {
            root: null,
            rootMargin: `${threshold}px`,
            threshold: 0
        };
        const observer = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    dotNetHelper.invokeMethodAsync('OnIntersect');
                }
            });
        }, options);
        observer.observe(sentinelElement);
        this.observers.set(sentinelElement, observer);
    },
    disconnect(sentinelElement) {
        const observer = this.observers.get(sentinelElement);
        if (observer) {
            observer.disconnect();
            this.observers.delete(sentinelElement);
        }
    }
};
// ========================================================================
// FILE DOWNLOAD HELPER
// ========================================================================
function downloadFileFromBase64(base64, fileName, mimeType) {
    const byteCharacters = atob(base64);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: mimeType });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
    URL.revokeObjectURL(link.href);
}
const chartJsInterop = {
    charts: new Map(),
    getChartColors() {
        const isDark = document.documentElement.classList.contains('dark');
        return {
            textColor: 'rgb(148, 163, 184)',
            gridColor: isDark ? 'rgba(71, 85, 105, 0.6)' : 'rgb(241, 245, 249)',
            tooltipTitleColor: isDark ? 'rgb(241, 245, 249)' : 'rgb(30, 41, 59)',
            tooltipBodyColor: isDark ? 'rgb(148, 163, 184)' : 'rgb(100, 116, 139)',
            tooltipBgColor: isDark ? 'rgb(51, 65, 85)' : 'rgb(255, 255, 255)',
            tooltipBorderColor: isDark ? 'rgb(71, 85, 105)' : 'rgb(226, 232, 240)'
        };
    },
    formatValue(value) {
        return Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
            maximumSignificantDigits: 3,
            // Removed 'notation: 'compact'' as it's not supported with 'currency' style
        }).format(value);
    },
    createLineChart(canvasId, config) {
        const ctx = document.getElementById(canvasId);
        if (ctx === null) {
            return false;
        }
        this.destroyChart(canvasId);
        const colors = this.getChartColors();
        const formatValue = this.formatValue;
        const chart = new Chart(ctx, {
            type: 'line',
            data: config.data,
            options: {
                layout: {
                    padding: 20,
                },
                scales: {
                    y: {
                        display: config.showYAxis || false,
                        beginAtZero: true,
                        suggestedMin: config.suggestedMin,
                        suggestedMax: config.suggestedMax,
                        border: { display: false },
                        ticks: {
                            maxTicksLimit: 5,
                            callback: (value) => formatValue(typeof value === 'number' ? value : parseFloat(value)),
                            color: colors.textColor
                        },
                        grid: {
                            color: colors.gridColor
                        }
                    },
                    x: {
                        type: config.useTimeScale ? 'time' : 'category',
                        time: config.useTimeScale ? {
                            parser: 'MM-dd-yyyy',
                            unit: 'month',
                            displayFormats: { month: 'MMM yy' }
                        } : undefined,
                        border: { display: false },
                        grid: { display: false },
                        ticks: {
                            maxTicksLimit: 7,
                            maxRotation: 0,
                            minRotation: 0,
                            color: colors.textColor
                        }
                    },
                },
                plugins: {
                    tooltip: {
                        callbacks: {
                            title: () => false,
                            label: (context) => formatValue(context.parsed.y),
                        },
                        bodyColor: colors.tooltipBodyColor,
                        backgroundColor: colors.tooltipBgColor,
                        borderColor: colors.tooltipBorderColor,
                        borderWidth: 1,
                        displayColors: false,
                        mode: 'nearest',
                        intersect: false,
                        position: 'nearest',
                        caretSize: 0,
                        caretPadding: 20,
                        cornerRadius: 8,
                        padding: 8
                    },
                    legend: { display: false },
                },
                interaction: {
                    intersect: false,
                    mode: 'nearest',
                },
                animation: config.disableAnimation ? false : undefined,
                maintainAspectRatio: false,
                resizeDelay: 200,
            },
        });
        this.charts.set(canvasId, chart);
        return true;
    },
    createBarChart(canvasId, config) {
        const ctx = document.getElementById(canvasId);
        if (ctx === null) {
            return false;
        }
        this.destroyChart(canvasId);
        const colors = this.getChartColors();
        const formatValue = this.formatValue;
        const chart = new Chart(ctx, {
            type: 'bar',
            data: config.data,
            options: {
                layout: {
                    padding: {
                        top: 12,
                        bottom: 16,
                        left: 20,
                        right: 20,
                    },
                },
                scales: {
                    y: {
                        border: { display: false },
                        ticks: {
                            maxTicksLimit: 5,
                            callback: (value) => formatValue(typeof value === 'number' ? value : parseFloat(value)),
                            color: colors.textColor,
                        },
                        grid: {
                            color: colors.gridColor,
                        },
                    },
                    x: {
                        type: config.useTimeScale ? 'time' : 'category',
                        time: config.useTimeScale ? {
                            parser: 'MM-dd-yyyy',
                            unit: 'month',
                            displayFormats: { month: 'MMM yy' },
                        } : undefined,
                        border: { display: false },
                        grid: { display: false },
                        ticks: { color: colors.textColor },
                    },
                },
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            title: () => false,
                            label: (context) => formatValue(context.parsed.y),
                        },
                        bodyColor: colors.tooltipBodyColor,
                        backgroundColor: colors.tooltipBgColor,
                        borderColor: colors.tooltipBorderColor,
                        borderWidth: 1,
                        displayColors: false,
                        cornerRadius: 8,
                        padding: 8
                    },
                },
                interaction: {
                    intersect: false,
                    mode: 'nearest',
                },
                animation: { duration: 500 },
                maintainAspectRatio: false,
                resizeDelay: 200,
            },
        });
        this.charts.set(canvasId, chart);
        return true;
    },
    createDoughnutChart(canvasId, config) {
        const ctx = document.getElementById(canvasId);
        if (ctx === null) {
            return false;
        }
        this.destroyChart(canvasId);
        const colors = this.getChartColors();
        const chart = new Chart(ctx, {
            type: 'doughnut',
            data: config.data,
            options: {
                cutout: config.cutout || '80%',
                layout: {
                    padding: 24,
                },
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        titleColor: colors.tooltipTitleColor,
                        bodyColor: colors.tooltipBodyColor,
                        backgroundColor: colors.tooltipBgColor,
                        borderColor: colors.tooltipBorderColor,
                        borderWidth: 1,
                        displayColors: false,
                        cornerRadius: 8,
                        padding: 8
                    },
                },
                interaction: {
                    intersect: false,
                    mode: 'nearest',
                },
                animation: { duration: 500 },
                maintainAspectRatio: false,
                resizeDelay: 200,
            },
        });
        this.charts.set(canvasId, chart);
        return true;
    },
    createPieChart(canvasId, config) {
        const ctx = document.getElementById(canvasId);
        if (ctx === null) {
            return false;
        }
        this.destroyChart(canvasId);
        const colors = this.getChartColors();
        const chart = new Chart(ctx, {
            type: 'pie',
            data: config.data,
            options: {
                layout: {
                    padding: 24,
                },
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        titleColor: colors.tooltipTitleColor,
                        bodyColor: colors.tooltipBodyColor,
                        backgroundColor: colors.tooltipBgColor,
                        borderColor: colors.tooltipBorderColor,
                        borderWidth: 1,
                        displayColors: false,
                        cornerRadius: 8,
                        padding: 8
                    },
                },
                interaction: {
                    intersect: false,
                    mode: 'nearest',
                },
                animation: { duration: 500 },
                maintainAspectRatio: false,
                resizeDelay: 200,
            },
        });
        this.charts.set(canvasId, chart);
        return true;
    },
    updateChart(canvasId, newData) {
        const chart = this.charts.get(canvasId);
        if (chart) {
            chart.data = newData;
            chart.update();
        }
    },
    destroyChart(canvasId) {
        const chart = this.charts.get(canvasId);
        if (chart) {
            chart.destroy();
            this.charts.delete(canvasId);
        }
    },
    updateChartTheme(canvasId) {
        var _a, _b, _c, _d;
        const chart = this.charts.get(canvasId);
        if (chart) {
            const colors = this.getChartColors();
            const typedChart = chart;
            if ((_a = typedChart.options.scales) === null || _a === void 0 ? void 0 : _a.y) {
                if (typedChart.options.scales.y.ticks) {
                    typedChart.options.scales.y.ticks.color = colors.textColor;
                }
                if (typedChart.options.scales.y.grid) {
                    typedChart.options.scales.y.grid.color = colors.gridColor;
                }
            }
            if ((_c = (_b = typedChart.options.scales) === null || _b === void 0 ? void 0 : _b.x) === null || _c === void 0 ? void 0 : _c.ticks) {
                typedChart.options.scales.x.ticks.color = colors.textColor;
            }
            if ((_d = typedChart.options.plugins) === null || _d === void 0 ? void 0 : _d.tooltip) {
                typedChart.options.plugins.tooltip.titleColor = colors.tooltipTitleColor;
                typedChart.options.plugins.tooltip.bodyColor = colors.tooltipBodyColor;
                typedChart.options.plugins.tooltip.backgroundColor = colors.tooltipBgColor;
                typedChart.options.plugins.tooltip.borderColor = colors.tooltipBorderColor;
            }
            typedChart.update('none');
        }
    },
    updateAllChartsTheme() {
        this.charts.forEach((_chart, canvasId) => {
            this.updateChartTheme(canvasId);
        });
    }
};
// ========================================================================
// WINDOW GLOBAL EXPORTS
// ========================================================================
// Export to window for Blazor interop
window.themeManager = themeManager;
window.getElementRect = getElementRect;
window.sidebarManager = sidebarManager;
window.cssHelper = cssHelper;
window.clickOutsideHelper = clickOutsideHelper;
window.dropdownManager = dropdownManager;
window.modalManager = modalManager;
window.datepickerManager = datepickerManager;
window.commandPaletteManager = commandPaletteManager;
window.infiniteScrollManager = infiniteScrollManager;
window.downloadFileFromBase64 = downloadFileFromBase64;
window.chartJsInterop = chartJsInterop;
//# sourceMappingURL=site.js.map