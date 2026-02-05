/**
 * MudBlazor .NET 10 Compatibility Polyfill
 * This polyfill addresses the breaking changes in Blazor JS interop for .NET 10
 * MUST load AFTER MudBlazor.min.js to augment the existing mudElementRef object
 */

interface DotNetObjectReference {
    invokeMethodAsync(methodName: string, ...args: unknown[]): Promise<unknown>;
}

interface MudElementRef {
    addOnBlurEvent?: (element: HTMLElement | null, dotNetRef: DotNetObjectReference) => void;
    addOnFocusEvent?: (element: HTMLElement | null, dotNetRef: DotNetObjectReference) => void;
    saveFocus?: (element: HTMLElement | null) => void;
    select?: (element: HTMLInputElement | HTMLTextAreaElement | null) => void;
    selectRange?: (element: HTMLInputElement | HTMLTextAreaElement | null, start: number, end: number) => void;
}

interface Window {
    mudElementRef: MudElementRef;
}

(function (): void {
    'use strict';

    console.log('🔧 MudBlazor .NET 10 polyfill starting...');

    // Ensure mudElementRef exists (MudBlazor should have created it)
    if (typeof window.mudElementRef === 'undefined') {
        console.warn('⚠️ mudElementRef not found - creating it (MudBlazor may not have loaded yet)');
        window.mudElementRef = {} as MudElementRef;
    } else {
        console.log('✓ mudElementRef found');
    }

    // Polyfill for addOnBlurEvent - critical for MudInput
    if (typeof window.mudElementRef.addOnBlurEvent !== 'function') {
        console.log('➕ Adding addOnBlurEvent polyfill');
        window.mudElementRef.addOnBlurEvent = function (
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
    } else {
        console.log('✓ addOnBlurEvent already exists');
    }

    // Polyfill for addOnFocusEvent
    if (typeof window.mudElementRef.addOnFocusEvent !== 'function') {
        console.log('➕ Adding addOnFocusEvent polyfill');
        window.mudElementRef.addOnFocusEvent = function (
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
    } else {
        console.log('✓ addOnFocusEvent already exists');
    }

    // Polyfill for saveFocus
    if (typeof window.mudElementRef.saveFocus !== 'function') {
        console.log('➕ Adding saveFocus polyfill');
        window.mudElementRef.saveFocus = function (element: HTMLElement | null): void {
            if (element !== null && typeof element.focus === 'function') {
                try {
                    element.focus();
                } catch (error) {
                    console.warn('MudBlazor polyfill: Error saving focus', error);
                }
            }
        };
    } else {
        console.log('✓ saveFocus already exists');
    }

    // Polyfill for select
    if (typeof window.mudElementRef.select !== 'function') {
        console.log('➕ Adding select polyfill');
        window.mudElementRef.select = function (element: HTMLInputElement | HTMLTextAreaElement | null): void {
            if (element !== null && typeof element.select === 'function') {
                try {
                    element.select();
                } catch (error) {
                    console.warn('MudBlazor polyfill: Error selecting element', error);
                }
            }
        };
    } else {
        console.log('✓ select already exists');
    }

    // Polyfill for selectRange
    if (typeof window.mudElementRef.selectRange !== 'function') {
        console.log('➕ Adding selectRange polyfill');
        window.mudElementRef.selectRange = function (
            element: HTMLInputElement | HTMLTextAreaElement | null,
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
    } else {
        console.log('✓ selectRange already exists');
    }

    // Log final status
    console.log('✅ MudBlazor .NET 10 compatibility polyfill loaded successfully');
    console.log('📋 mudElementRef functions available:', Object.keys(window.mudElementRef || {}));
})();
