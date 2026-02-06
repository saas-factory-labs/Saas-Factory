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

(function (): void {
    'use strict';

    console.log('🔧 MudBlazor .NET 10 polyfill starting...');

    // Ensure mudElementRef exists (MudBlazor should have created it)
    if ((globalThis as unknown as Window).mudElementRef === undefined) {
        console.warn('⚠️ mudElementRef not found - creating it (MudBlazor may not have loaded yet)');
        (globalThis as unknown as Window).mudElementRef = {} as MudElementRef;
    } else {
        console.log('✓ mudElementRef found');
    }

    // Polyfill for addOnBlurEvent - critical for MudInput
    if (typeof (globalThis as unknown as Window).mudElementRef.addOnBlurEvent === 'function') {
        console.log('✓ addOnBlurEvent already exists');
    } else {
        console.log('➕ Adding addOnBlurEvent polyfill');
        (globalThis as unknown as Window).mudElementRef.addOnBlurEvent = function (
            element: HTMLElement | null,
            dotNetRef: DotNetObjectReference
        ): void {
            if (element === null) {
                console.warn('MudBlazor polyfill: addOnBlurEvent called with null element');
                return;
            }

            console.log('📌 Attaching blur event to element:', element);
            element.addEventListener('blur', async function (): Promise<void> {
                try {
                    if (dotNetRef && typeof dotNetRef.invokeMethodAsync === 'function') {
                        await dotNetRef.invokeMethodAsync('OnBlur');
                    }
                } catch (error) {
                    console.warn('MudBlazor polyfill: Error invoking OnBlur', error);
                }
            });
        };
    }

    // Polyfill for addOnFocusEvent
    if (typeof (globalThis as unknown as Window).mudElementRef.addOnFocusEvent === 'function') {
        console.log('✓ addOnFocusEvent already exists');
    } else {
        console.log('➕ Adding addOnFocusEvent polyfill');
        (globalThis as unknown as Window).mudElementRef.addOnFocusEvent = function (
            element: HTMLElement | null,
            dotNetRef: DotNetObjectReference
        ): void {
            if (element === null) {
                console.warn('MudBlazor polyfill: addOnFocusEvent called with null element');
                return;
            }

            element.addEventListener('focus', async function (): Promise<void> {
                try {
                    if (dotNetRef && typeof dotNetRef.invokeMethodAsync === 'function') {
                        await dotNetRef.invokeMethodAsync('OnFocus');
                    }
                } catch (error) {
                    console.warn('MudBlazor polyfill: Error invoking OnFocus', error);
                }
            });
        };
    }

    // Polyfill for saveFocus
    if (typeof (globalThis as unknown as Window).mudElementRef.saveFocus === 'function') {
        console.log('✓ saveFocus already exists');
    } else {
        console.log('➕ Adding saveFocus polyfill');
        (globalThis as unknown as Window).mudElementRef.saveFocus = function (element: HTMLElement | null): void {
            if (element !== null && typeof element.focus === 'function') {
                try {
                    element.focus();
                } catch (error) {
                    console.warn('MudBlazor polyfill: Error saving focus', error);
                }
            }
        };
    }

    // Polyfill for select
    if (typeof (globalThis as unknown as Window).mudElementRef.select === 'function') {
        console.log('✓ select already exists');
    } else {
        console.log('➕ Adding select polyfill');
        (globalThis as unknown as Window).mudElementRef.select = function (element: InputElement | null): void {
            if (element !== null && typeof element.select === 'function') {
                try {
                    element.select();
                } catch (error) {
                    console.warn('MudBlazor polyfill: Error selecting element', error);
                }
            }
        };
    }

    // Polyfill for selectRange
    if (typeof (globalThis as unknown as Window).mudElementRef.selectRange === 'function') {
        console.log('✓ selectRange already exists');
    } else {
        console.log('➕ Adding selectRange polyfill');
        (globalThis as unknown as Window).mudElementRef.selectRange = function (
            element: InputElement | null,
            start: number,
            end: number
        ): void {
            if (element !== null && typeof element.setSelectionRange === 'function') {
                try {
                    element.setSelectionRange(start, end);
                } catch (error) {
                    console.warn('MudBlazor polyfill: Error selecting range', error);
                }
            }
        };
    }

    // Log final status
    console.log('✅ MudBlazor .NET 10 compatibility polyfill loaded successfully');
    console.log('📋 mudElementRef functions available:', Object.keys((globalThis as unknown as Window).mudElementRef || {}));
})();
