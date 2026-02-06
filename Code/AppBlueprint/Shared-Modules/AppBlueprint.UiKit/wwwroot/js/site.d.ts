/**
 * AppBlueprint UiKit - Site-wide JavaScript interop module
 * Provides comprehensive Blazor interoperability for UI components
 */
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
    handlers: Map<HTMLElement, {
        clickHandler: EventListener;
        keyHandler: (event: KeyboardEvent) => void;
    }>;
    initialize(dropdown: HTMLElement, trigger: HTMLElement, dotNetHelper: DotNetObjectReference): void;
    cleanup(dropdown: HTMLElement): void;
}
interface ModalManager {
    handlers: Map<HTMLElement, {
        clickHandler: EventListener;
        keyHandler: (event: KeyboardEvent) => void;
    }>;
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
declare const themeManager: ThemeManager;
declare function getElementRect(selector: string): ElementRect | null;
declare const sidebarManager: SidebarManager;
declare const cssHelper: CssHelper;
declare const clickOutsideHelper: ClickOutsideHelper;
declare const dropdownManager: DropdownManager;
declare const modalManager: ModalManager;
declare const datepickerManager: DatepickerManager;
declare const commandPaletteManager: CommandPaletteManager;
declare const infiniteScrollManager: InfiniteScrollManager;
declare function downloadFileFromBase64(base64: string, fileName: string, mimeType: string): void;
type ChartConstructor = new (context: HTMLCanvasElement, config: unknown) => {
    data: unknown;
    options: {
        scales?: {
            y?: {
                ticks?: {
                    color?: string;
                };
                grid?: {
                    color?: string;
                };
            };
            x?: {
                ticks?: {
                    color?: string;
                };
            };
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
declare const chartJsInterop: ChartJsInterop;
//# sourceMappingURL=site.d.ts.map