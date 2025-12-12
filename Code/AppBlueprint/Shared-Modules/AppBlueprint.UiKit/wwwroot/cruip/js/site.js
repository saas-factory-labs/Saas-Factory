// Theme management
window.themeManager = {
    toggleDarkMode: function () {
        const html = document.documentElement;
        if (html.classList.contains('dark')) {
            html.classList.remove('dark');
            html.style.colorScheme = 'light';
            localStorage.setItem('theme', 'light');
        } else {
            html.classList.add('dark');
            html.style.colorScheme = 'dark';
            localStorage.setItem('theme', 'dark');
        }
    },

    isDarkMode: function () {
        return document.documentElement.classList.contains('dark');
    },

    getTheme: function () {
        return localStorage.getItem('theme') || 'system';
    },

    setTheme: function (theme) {
        const html = document.documentElement;
        localStorage.setItem('theme', theme);

        if (theme === 'dark') {
            html.classList.add('dark');
            html.style.colorScheme = 'dark';
        } else if (theme === 'light') {
            html.classList.remove('dark');
            html.style.colorScheme = 'light';
        } else {
            // System preference
            if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
                html.classList.add('dark');
                html.style.colorScheme = 'dark';
            } else {
                html.classList.remove('dark');
                html.style.colorScheme = 'light';
            }
        }
    }
};

// Get element position for onboarding tour
window.getElementRect = function (selector) {
    const element = document.querySelector(selector);
    if (!element) return null;
    const rect = element.getBoundingClientRect();
    return {
        top: rect.top + window.scrollY,
        left: rect.left + window.scrollX,
        right: rect.right + window.scrollX,
        bottom: rect.bottom + window.scrollY,
        width: rect.width,
        height: rect.height
    };
};

// Sidebar management
window.sidebarManager = {
    toggleExpanded: function () {
        const body = document.querySelector('body');
        if (body.classList.contains('sidebar-expanded')) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
        } else {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
        }
    },

    isExpanded: function () {
        return document.querySelector('body').classList.contains('sidebar-expanded');
    }
};

// CSS custom property helpers
window.cssHelper = {
    getCssVariable: function (variable) {
        return getComputedStyle(document.documentElement).getPropertyValue(variable).trim();
    },

    adjustColorOpacity: function (color, opacity) {
        if (color.startsWith('#')) {
            return this.adjustHexOpacity(color, opacity);
        } else if (color.startsWith('hsl')) {
            return this.adjustHSLOpacity(color, opacity);
        } else if (color.startsWith('oklch')) {
            return this.adjustOKLCHOpacity(color, opacity);
        }
        return color;
    },

    adjustHexOpacity: function (hex, opacity) {
        const r = parseInt(hex.slice(1, 3), 16);
        const g = parseInt(hex.slice(3, 5), 16);
        const b = parseInt(hex.slice(5, 7), 16);
        return `rgba(${r}, ${g}, ${b}, ${opacity})`;
    },

    adjustHSLOpacity: function (hsl, opacity) {
        return hsl.replace(')', `, ${opacity})`).replace('hsl', 'hsla');
    },

    adjustOKLCHOpacity: function (oklch, opacity) {
        return oklch.replace(')', ` / ${opacity})`);
    }
};

// Click outside handler
window.clickOutsideHelper = {
    register: function (element, dotNetHelper, methodName) {
        const handler = (event) => {
            if (element && !element.contains(event.target)) {
                dotNetHelper.invokeMethodAsync(methodName);
            }
        };
        document.addEventListener('click', handler);
        return handler;
    },

    unregister: function (handler) {
        document.removeEventListener('click', handler);
    }
};

// Dropdown management
window.dropdownManager = {
    handlers: new Map(),

    initialize: function (dropdown, trigger, dotNetHelper) {
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

    cleanup: function (dropdown) {
        const handlers = this.handlers.get(dropdown);
        if (handlers) {
            document.removeEventListener('click', handlers.clickHandler);
            document.removeEventListener('keydown', handlers.keyHandler);
            this.handlers.delete(dropdown);
        }
    }
};

// Modal management
window.modalManager = {
    handlers: new Map(),

    initialize: function (modalContent, dotNetHelper) {
        const clickHandler = (event) => {
            if (!modalContent.contains(event.target)) {
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

        this.handlers.set(modalContent, { clickHandler, keyHandler });
    },

    initializeSearch: function (modalContent, searchInput, dotNetHelper) {
        // Initialize modal handlers
        this.initialize(modalContent, dotNetHelper);

        // Focus search input
        if (searchInput) {
            setTimeout(() => searchInput.focus(), 100);
        }
    },

    cleanup: function (modalContent) {
        const handlers = this.handlers.get(modalContent);
        if (handlers) {
            document.removeEventListener('click', handlers.clickHandler);
            document.removeEventListener('keydown', handlers.keyHandler);
            this.handlers.delete(modalContent);
        }
    }
};

// Datepicker management using flatpickr
window.datepickerManager = {
    instances: {},
    initialize: function (input, align) {
        if (!window.flatpickr) {
            console.error('Flatpickr library not loaded');
            return;
        }

        // Destroy existing instance if any
        if (this.instances[input.id]) {
            this.instances[input.id].destroy();
        }

        const customClass = align || '';

        // Initialize flatpickr with same config as Vue template
        this.instances[input.id] = flatpickr(input, {
            mode: 'range',
            static: true,
            monthSelectorType: 'static',
            dateFormat: 'M j, Y',
            defaultDate: [new Date(new Date().setDate(new Date().getDate() - 6)), new Date()],
            prevArrow: '<svg class="fill-current" width="7" height="11" viewBox="0 0 7 11"><path d="M5.4 10.8l1.4-1.4-4-4 4-4L5.4 0 0 5.4z" /></svg>',
            nextArrow: '<svg class="fill-current" width="7" height="11" viewBox="0 0 7 11"><path d="M1.4 10.8L0 9.4l4-4-4-4L1.4 0l5.4 5.4z" /></svg>',
            onReady: function (selectedDates, dateStr, instance) {
                instance.element.value = dateStr.replace('to', '-');
                if (customClass) {
                    instance.calendarContainer.classList.add(`flatpickr-${customClass}`);
                }
            },
            onChange: function (selectedDates, dateStr, instance) {
                instance.element.value = dateStr.replace('to', '-');
            }
        });
    }
};

// Command palette management
window.commandPaletteManager = {
    dotNetHelper: null,

    initialize: function (dotNetHelper) {
        this.dotNetHelper = dotNetHelper;

        // Listen for Ctrl+K keyboard shortcut
        document.addEventListener('keydown', (event) => {
            if ((event.ctrlKey || event.metaKey) && event.key === 'k') {
                event.preventDefault();
                if (this.dotNetHelper) {
                    this.dotNetHelper.invokeMethodAsync('OpenFromJS');
                }
            }
        });
    }
};

// Infinite scroll management with Intersection Observer
window.infiniteScrollManager = {
    observers: new Map(),

    initialize: function (sentinelElement, dotNetHelper, threshold) {
        if (!sentinelElement) return;

        const options = {
            root: null,
            rootMargin: `${threshold}px`,
            threshold: 0
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    dotNetHelper.invokeMethodAsync('OnIntersect');
                }
            });
        }, options);

        observer.observe(sentinelElement);
        this.observers.set(sentinelElement, observer);
    },

    disconnect: function (sentinelElement) {
        const observer = this.observers.get(sentinelElement);
        if (observer) {
            observer.disconnect();
            this.observers.delete(sentinelElement);
        }
    }
};

// File download helper
window.downloadFileFromBase64 = function (base64, fileName, mimeType) {
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
};

// Chart.js v4 Interop for Blazor
window.chartJsInterop = {
    charts: new Map(),

    // Get chart colors based on dark mode
    getChartColors: function () {
        const isDark = document.documentElement.classList.contains('dark');
        return {
            textColor: isDark ? 'rgb(148, 163, 184)' : 'rgb(148, 163, 184)',
            gridColor: isDark ? 'rgba(71, 85, 105, 0.6)' : 'rgb(241, 245, 249)',
            tooltipTitleColor: isDark ? 'rgb(241, 245, 249)' : 'rgb(30, 41, 59)',
            tooltipBodyColor: isDark ? 'rgb(148, 163, 184)' : 'rgb(100, 116, 139)',
            tooltipBgColor: isDark ? 'rgb(51, 65, 85)' : 'rgb(255, 255, 255)',
            tooltipBorderColor: isDark ? 'rgb(71, 85, 105)' : 'rgb(226, 232, 240)'
        };
    },

    // Format value for tooltips
    formatValue: function (value) {
        return Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
            maximumSignificantDigits: 3,
            notation: 'compact',
        }).format(value);
    },

    // Create Line Chart
    createLineChart: function (canvasId, config) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        // Destroy existing chart if any
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
                            callback: (value) => formatValue(value),
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
                        display: config.showXAxis || false,
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

    // Create Bar Chart
    createBarChart: function (canvasId, config) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

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
                            callback: (value) => formatValue(value),
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

    // Create Doughnut Chart
    createDoughnutChart: function (canvasId, config) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

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

    // Create Pie Chart
    createPieChart: function (canvasId, config) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

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

    // Update chart data
    updateChart: function (canvasId, newData) {
        const chart = this.charts.get(canvasId);
        if (chart) {
            chart.data = newData;
            chart.update();
        }
    },

    // Destroy chart
    destroyChart: function (canvasId) {
        const chart = this.charts.get(canvasId);
        if (chart) {
            chart.destroy();
            this.charts.delete(canvasId);
        }
    },

    // Update chart colors when theme changes
    updateChartTheme: function (canvasId) {
        const chart = this.charts.get(canvasId);
        if (chart) {
            const colors = this.getChartColors();

            if (chart.options.scales?.y) {
                chart.options.scales.y.ticks.color = colors.textColor;
                chart.options.scales.y.grid.color = colors.gridColor;
            }
            if (chart.options.scales?.x) {
                chart.options.scales.x.ticks.color = colors.textColor;
            }
            if (chart.options.plugins?.tooltip) {
                chart.options.plugins.tooltip.titleColor = colors.tooltipTitleColor;
                chart.options.plugins.tooltip.bodyColor = colors.tooltipBodyColor;
                chart.options.plugins.tooltip.backgroundColor = colors.tooltipBgColor;
                chart.options.plugins.tooltip.borderColor = colors.tooltipBorderColor;
            }

            chart.update('none');
        }
    },

    // Update all charts theme
    updateAllChartsTheme: function () {
        this.charts.forEach((chart, canvasId) => {
            this.updateChartTheme(canvasId);
        });
    }
};
