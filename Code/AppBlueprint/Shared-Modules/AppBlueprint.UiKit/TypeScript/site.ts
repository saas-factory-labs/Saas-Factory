/**
 * AppBlueprint UiKit - Site-wide JavaScript interop module
 * Provides comprehensive Blazor interoperability for UI components
 */

// ========================================================================
// TYPE DEFINITIONS
// ========================================================================

interface DotNetObjectReference {
    invokeMethodAsync(methodName: string, ...args: unknown[]): Promise<unknown>;
}

interface ElementRect {
    top: number;
    left: number;
    right: number;
    bottom: number;
    width: number;
    height: number;
}

interface ThemeManager {
    toggleDarkMode(): void;
    isDarkMode(): boolean;
    getTheme(): string;
    setTheme(theme: 'dark' | 'light' | 'system'): void;
}

interface SidebarManager {
    toggleExpanded(): void;
    expand(): void;
    collapse(): void;
    isExpanded(): boolean;
}

interface CssHelper {
    getCssVariable(variable: string): string;
    adjustColorOpacity(color: string, opacity: number): string;
    adjustHexOpacity(hex: string, opacity: number): string;
    adjustHSLOpacity(hsl: string, opacity: number): string;
    adjustOKLCHOpacity(oklch: string, opacity: number): string;
}

interface ClickOutsideHelper {
    register(element: HTMLElement, dotNetHelper: DotNetObjectReference, methodName: string): EventListener;
    unregister(handler: EventListener): void;
}

interface DropdownManager {
    handlers: Map<HTMLElement, { clickHandler: EventListener; keyHandler: (event: KeyboardEvent) => void }>; // Changed EventListener to (event: KeyboardEvent) => void
    initialize(dropdown: HTMLElement, trigger: HTMLElement, dotNetHelper: DotNetObjectReference): void;
    cleanup(dropdown: HTMLElement): void;
}

interface ModalManager {
    handlers: Map<HTMLElement, { clickHandler: EventListener; keyHandler: (event: KeyboardEvent) => void }>;
    _registerClickOutsideHandler(targetElement: HTMLElement, dotNetHelper: DotNetObjectReference): EventListener;
    _registerEscapeKeyHandler(dotNetHelper: DotNetObjectReference): (event: KeyboardEvent) => void;
    initialize(modalContent: HTMLElement, dotNetHelper: DotNetObjectReference): void;
    initializeSearch(modalContent: HTMLElement, searchInput: HTMLInputElement, dotNetHelper: DotNetObjectReference): void;
    cleanup(modalContent: HTMLElement): void;
}

interface DatepickerManager {
    instances: Record<string, unknown>;
    initialize(input: HTMLInputElement, align?: string): void;
}

interface CommandPaletteManager {
    dotNetHelper: DotNetObjectReference | null;
    initialize(dotNetHelper: DotNetObjectReference): void;
}

interface InfiniteScrollManager {
    observers: Map<Element, IntersectionObserver>;
    initialize(sentinelElement: Element, dotNetHelper: DotNetObjectReference, threshold: number): void;
    disconnect(sentinelElement: Element): void;
}

interface ChartColors {
    textColor: string;
    gridColor: string;
    tooltipTitleColor: string;
    tooltipBodyColor: string;
    tooltipBgColor: string;
    tooltipBorderColor: string;
}

interface ChartConfig {
    data: unknown;
    showYAxis?: boolean;
    showXAxis?: boolean;
    suggestedMin?: number;
    suggestedMax?: number;
    useTimeScale?: boolean;
    disableAnimation?: boolean;
    cutout?: string;
}

interface ChartJsInterop {
    charts: Map<string, unknown>;
    getChartColors(): ChartColors;
    formatValue(value: number): string;
    createLineChart(canvasId: string, config: ChartConfig): boolean;
    createBarChart(canvasId: string, config: ChartConfig): boolean;
    createDoughnutChart(canvasId: string, config: ChartConfig): boolean;
    createPieChart(canvasId: string, config: ChartConfig): boolean;
    updateChart(canvasId: string, newData: unknown): void;
    destroyChart(canvasId: string): void;
    updateChartTheme(canvasId: string): void;
    updateAllChartsTheme(): void;
}

// ========================================================================
// THEME MANAGEMENT
// ========================================================================

const themeManager: ThemeManager = {
    toggleDarkMode(): void {
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

    isDarkMode(): boolean {
        return document.documentElement.classList.contains('dark');
    },

    getTheme(): string {
        return localStorage.getItem('theme') || 'system';
    },

    setTheme(theme: 'dark' | 'light' | 'system'): void {
        const html = document.documentElement;
        localStorage.setItem('theme', theme);

        const isDark = theme === 'dark' || (theme === 'system' && globalThis.matchMedia('(prefers-color-scheme: dark)').matches);
        
        if (isDark) {
            html.classList.add('dark');
            html.style.colorScheme = 'dark';
        } else {
            html.classList.remove('dark');
            html.style.colorScheme = 'light';
        }
    }
};

// ========================================================================
// ELEMENT POSITION UTILITIES
// ========================================================================

function getElementRect(selector: string): ElementRect | null {
    const element = document.querySelector<HTMLElement>(selector);
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

const sidebarManager: SidebarManager = {
    toggleExpanded(): void {
        console.log('sidebarManager.toggleExpanded called');
        const body = document.querySelector<HTMLBodyElement>('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        
        const isExpanded = body.classList.contains('sidebar-expanded');
        
        if (isExpanded) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
            console.log('Sidebar collapsed');
        } else {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
            console.log('Sidebar expanded');
        }
    },
    
    expand(): void {
        console.log('sidebarManager.expand called');
        const body = document.querySelector<HTMLBodyElement>('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        
        if (body.classList.contains('sidebar-expanded')) {
            console.log('Sidebar already expanded');
        } else {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
            console.log('Sidebar expanded (was collapsed)');
        }
    },
    
    collapse(): void {
        console.log('sidebarManager.collapse called');
        const body = document.querySelector<HTMLBodyElement>('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        
        if (body.classList.contains('sidebar-expanded')) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
            console.log('Sidebar collapsed (was expanded)');
        } else {
            console.log('Sidebar already collapsed');
        }
    },

    isExpanded(): boolean {
        const body = document.querySelector<HTMLBodyElement>('body');
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

const cssHelper: CssHelper = {
    getCssVariable(variable: string): string {
        return getComputedStyle(document.documentElement).getPropertyValue(variable).trim();
    },

    adjustColorOpacity(color: string, opacity: number): string {
        if (color.startsWith('#')) {
            return this.adjustHexOpacity(color, opacity);
        } else if (color.startsWith('hsl')) {
            return this.adjustHSLOpacity(color, opacity);
        } else if (color.startsWith('oklch')) {
            return this.adjustOKLCHOpacity(color, opacity);
        }
        return color;
    },

    adjustHexOpacity(hex: string, opacity: number): string {
        const r = Number.parseInt(hex.slice(1, 3), 16);
        const g = Number.parseInt(hex.slice(3, 5), 16);
        const b = Number.parseInt(hex.slice(5, 7), 16);
        return `rgba(${r}, ${g}, ${b}, ${opacity})`;
    },

    adjustHSLOpacity(hsl: string, opacity: number): string {
        return hsl.replace(')', `, ${opacity})`).replace('hsl', 'hsla');
    },

    adjustOKLCHOpacity(oklch: string, opacity: number): string {
        return oklch.replace(')', ` / ${opacity})`);
    }
};

// ========================================================================
// CLICK OUTSIDE HANDLER
// ========================================================================

const clickOutsideHelper: ClickOutsideHelper = {
    register(element: HTMLElement, dotNetHelper: DotNetObjectReference, methodName: string): EventListener {
        const handler = (event: Event): void => {
            if (element !== null && !element.contains(event.target as Node)) {
                dotNetHelper.invokeMethodAsync(methodName);
            }
        };
        document.addEventListener('click', handler);
        return handler;
    },

    unregister(handler: EventListener): void {
        document.removeEventListener('click', handler);
    }
};

// ========================================================================
// DROPDOWN MANAGEMENT
// ========================================================================

const dropdownManager: DropdownManager = {
    handlers: new Map(),

    initialize(dropdown: HTMLElement, trigger: HTMLElement, dotNetHelper: DotNetObjectReference): void {
        const clickHandler = (event: Event): void => {
            if (!dropdown.contains(event.target as Node) && !trigger.contains(event.target as Node)) {
                dotNetHelper.invokeMethodAsync('CloseFromJS');
            }
        };

        const keyHandler = (event: KeyboardEvent): void => {
            if (event.key === 'Escape') {
                dotNetHelper.invokeMethodAsync('CloseFromJS');
            }
        };

        document.addEventListener('click', clickHandler);
        document.addEventListener('keydown', keyHandler);

        this.handlers.set(dropdown, { clickHandler, keyHandler });
    },

    cleanup(dropdown: HTMLElement): void {
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

const modalManager: ModalManager = {
    handlers: new Map(),

    _registerClickOutsideHandler(targetElement: HTMLElement, dotNetHelper: DotNetObjectReference): EventListener {
        return (event: Event): void => {
            if (!targetElement.contains(event.target as Node)) {
                dotNetHelper.invokeMethodAsync('CloseFromJS');
            }
        };
    },

    _registerEscapeKeyHandler(dotNetHelper: DotNetObjectReference): (event: KeyboardEvent) => void {
        return (event: KeyboardEvent): void => {
            if (event.key === 'Escape') {
                dotNetHelper.invokeMethodAsync('CloseFromJS');
            }
        };
    },

    initialize(modalContent: HTMLElement, dotNetHelper: DotNetObjectReference): void {
        const clickHandler = this._registerClickOutsideHandler(modalContent, dotNetHelper);
        const keyHandler = this._registerEscapeKeyHandler(dotNetHelper);
        
        document.addEventListener('click', clickHandler);
        document.addEventListener('keydown', keyHandler);
        
        this.handlers.set(modalContent, { clickHandler, keyHandler });
    },

    initializeSearch(modalContent: HTMLElement, searchInput: HTMLInputElement, dotNetHelper: DotNetObjectReference): void {
        const clickHandler = this._registerClickOutsideHandler(modalContent, dotNetHelper);
        const keyHandler = this._registerEscapeKeyHandler(dotNetHelper);
        
        document.addEventListener('click', clickHandler);
        document.addEventListener('keydown', keyHandler);
        
        this.handlers.set(modalContent, { clickHandler, keyHandler });

        // Focus search input
        if (searchInput !== null) {
            setTimeout(() => searchInput.focus(), 100);
        }
    },

    cleanup(modalContent: HTMLElement): void {
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

const datepickerManager: DatepickerManager = {
    instances: {},
    initialize(input: HTMLInputElement, align?: string): void {
        if ((globalThis as unknown as { flatpickr?: unknown }).flatpickr === undefined) {
            console.error('Flatpickr library not loaded');
            return;
        }

        // Destroy existing instance if any
        if (this.instances[input.id] !== null) {
            (this.instances[input.id] as { destroy: () => void }).destroy();
        }

        const customClass = align || '';

        // Initialize flatpickr with same config as Vue template
        this.instances[input.id] = (globalThis as unknown as { flatpickr: (el: HTMLInputElement, config: unknown) => unknown }).flatpickr(input, {
            mode: 'range',
            static: true,
            monthSelectorType: 'static',
            dateFormat: 'M j, Y',
            defaultDate: [new Date(new Date().setDate(new Date().getDate() - 6)), new Date()],
            prevArrow: '<svg class="fill-current" width="7" height="11" viewBox="0 0 7 11"><path d="M5.4 10.8l1.4-1.4-4-4 4-4L5.4 0 0 5.4z" /></svg>',
            nextArrow: '<svg class="fill-current" width="7" height="11" viewBox="0 0 7 11"><path d="M1.4 10.8L0 9.4l4-4-4-4L1.4 0l5.4 5.4z" /></svg>',
            onReady(selectedDates: Date[], dateStr: string, instance: { element: HTMLInputElement; calendarContainer: HTMLElement }): void {
                instance.element.value = dateStr.replace('to', '-');
                if (customClass) {
                    instance.calendarContainer.classList.add(`flatpickr-${customClass}`);
                }
            },
            onChange(selectedDates: Date[], dateStr: string, instance: { element: HTMLInputElement }): void {
                instance.element.value = dateStr.replace('to', '-'); // Corrected typo here (Freplace -> replace)
            }
        });
    }
};

// ========================================================================
// COMMAND PALETTE MANAGEMENT
// ========================================================================

const commandPaletteManager: CommandPaletteManager = {
    dotNetHelper: null,

    initialize(dotNetHelper: DotNetObjectReference): void {
        this.dotNetHelper = dotNetHelper;

        // Listen for Ctrl+K keyboard shortcut
        document.addEventListener('keydown', (event: KeyboardEvent): void => {
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

const infiniteScrollManager: InfiniteScrollManager = {
    observers: new Map(),

    initialize(sentinelElement: Element, dotNetHelper: DotNetObjectReference, threshold: number): void {
        if (sentinelElement === null) {
            return;
        }

        const options: IntersectionObserverInit = {
            root: null,
            rootMargin: `${threshold}px`,
            threshold: 0
        };

        const observer = new IntersectionObserver((entries: IntersectionObserverEntry[]): void => {
            entries.forEach((entry: IntersectionObserverEntry): void => {
                if (entry.isIntersecting) {
                    dotNetHelper.invokeMethodAsync('OnIntersect');
                }
            });
        }, options);

        observer.observe(sentinelElement);
        this.observers.set(sentinelElement, observer);
    },

    disconnect(sentinelElement: Element): void {
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

function downloadFileFromBase64(base64: string, fileName: string, mimeType: string): void {
    const byteCharacters = atob(base64);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.codePointAt(i) || 0;
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: mimeType });

    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
    URL.revokeObjectURL(link.href);
}

// ========================================================================
// CHART.JS INTEROP
// ========================================================================

type ChartConstructor = new (context: HTMLCanvasElement, config: unknown) => {
    data: unknown;
    options: {
        scales?: {
            y?: { ticks?: { color?: string }; grid?: { color?: string } };
            x?: { ticks?: { color?: string } };
        };
        plugins?: {
            tooltip?: {
                titleColor?: string;
                bodyColor?: string;
                backgroundColor?: string;
                borderColor?: string;
            };
        };
    };
    update(mode?: string): void;
    destroy(): void;
};

declare const Chart: ChartConstructor;

const chartJsInterop: ChartJsInterop = {
    charts: new Map(),

    getChartColors(): ChartColors {
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

    formatValue(value: number): string {
        return Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
            maximumSignificantDigits: 3,
            // Removed 'notation: 'compact'' as it's not supported with 'currency' style
        }).format(value);
    },

    createLineChart(canvasId: string, config: ChartConfig): boolean {
        const ctx = document.getElementById(canvasId) as HTMLCanvasElement | null;
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
                            callback: (value: string | number) => formatValue(typeof value === 'number' ? value : Number.parseFloat(value)),
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
                            label: (context: { parsed: { y: number } }) => formatValue(context.parsed.y),
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

    createBarChart(canvasId: string, config: ChartConfig): boolean {
        const ctx = document.getElementById(canvasId) as HTMLCanvasElement | null;
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
                            callback: (value: string | number) => formatValue(typeof value === 'number' ? value : Number.parseFloat(value)),
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
                            label: (context: { parsed: { y: number } }) => formatValue(context.parsed.y),
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

    createDoughnutChart(canvasId: string, config: ChartConfig): boolean {
        const ctx = document.getElementById(canvasId) as HTMLCanvasElement | null;
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

    createPieChart(canvasId: string, config: ChartConfig): boolean {
        const ctx = document.getElementById(canvasId) as HTMLCanvasElement | null;
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

    updateChart(canvasId: string, newData: unknown): void {
        const chart = this.charts.get(canvasId) as { data: unknown; update: () => void } | undefined;
        if (chart) {
            chart.data = newData;
            chart.update();
        }
    },

    destroyChart(canvasId: string): void {
        const chart = this.charts.get(canvasId) as { destroy: () => void } | undefined;
        if (chart) {
            chart.destroy();
            this.charts.delete(canvasId);
        }
    },

    updateChartTheme(canvasId: string): void {
        const chart = this.charts.get(canvasId);
        if (chart) {
            const colors = this.getChartColors();
            const typedChart = chart as {
                options: {
                    scales?: {
                        y?: { ticks?: { color?: string }; grid?: { color?: string } };
                        x?: { ticks?: { color?: string } };
                    };
                    plugins?: {
                        tooltip?: {
                            titleColor?: string;
                            bodyColor?: string;
                            backgroundColor?: string;
                            borderColor?: string;
                        };
                    };
                };
                update: (mode?: string) => void;
            };

            if (typedChart.options.scales?.y) {
                if (typedChart.options.scales.y.ticks) {
                    typedChart.options.scales.y.ticks.color = colors.textColor;
                }
                if (typedChart.options.scales.y.grid) {
                    typedChart.options.scales.y.grid.color = colors.gridColor;
                }
            }
            if (typedChart.options.scales?.x?.ticks) {
                typedChart.options.scales.x.ticks.color = colors.textColor;
            }
            if (typedChart.options.plugins?.tooltip) {
                typedChart.options.plugins.tooltip.titleColor = colors.tooltipTitleColor;
                typedChart.options.plugins.tooltip.bodyColor = colors.tooltipBodyColor;
                typedChart.options.plugins.tooltip.backgroundColor = colors.tooltipBgColor;
                typedChart.options.plugins.tooltip.borderColor = colors.tooltipBorderColor;
            }

            typedChart.update('none');
        }
    },

    updateAllChartsTheme(): void {
        this.charts.forEach((_chart, canvasId) => {
            this.updateChartTheme(canvasId);
        });
    }
};

// ========================================================================
// GLOBAL EXPORTS
// ========================================================================

// Export to globalThis for Blazor interop
(globalThis as any).themeManager = themeManager;
(globalThis as any).getElementRect = getElementRect;
(globalThis as any).sidebarManager = sidebarManager;
(globalThis as any).cssHelper = cssHelper;
(globalThis as any).clickOutsideHelper = clickOutsideHelper;
(globalThis as any).dropdownManager = dropdownManager;
(globalThis as any).modalManager = modalManager;
(globalThis as any).datepickerManager = datepickerManager;
(globalThis as any).commandPaletteManager = commandPaletteManager;
(globalThis as any).infiniteScrollManager = infiniteScrollManager;
(globalThis as any).downloadFileFromBase64 = downloadFileFromBase64;
(globalThis as any).chartJsInterop = chartJsInterop;