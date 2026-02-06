/**
 * MudBlazor .NET 10 Compatibility Polyfill
 * This polyfill addresses the breaking changes in Blazor JS interop for .NET 10
 * MUST load AFTER MudBlazor.min.js to augment the existing mudElementRef object
 */
interface DotNetObjectReference {
    invokeMethodAsync(methodName: string, ...args: unknown[]): Promise<unknown>;
}
type InputElement = HTMLInputElement | HTMLTextAreaElement;
interface MudElementRef {
    addOnBlurEvent?: (element: HTMLElement | null, dotNetRef: DotNetObjectReference) => void;
    addOnFocusEvent?: (element: HTMLElement | null, dotNetRef: DotNetObjectReference) => void;
    saveFocus?: (element: HTMLElement | null) => void;
    select?: (element: InputElement | null) => void;
    selectRange?: (element: InputElement | null, start: number, end: number) => void;
}
interface Window {
    mudElementRef: MudElementRef;
}
//# sourceMappingURL=mudblazor-net10-polyfill.d.ts.map