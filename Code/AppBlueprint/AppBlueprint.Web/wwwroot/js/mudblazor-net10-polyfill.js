// MudBlazor .NET 10 Compatibility Polyfill
// This polyfill addresses the breaking changes in Blazor JS interop for .NET 10

(function () {
    'use strict';

    // Create the mudElementRef namespace if it doesn't exist
    if (typeof window.mudElementRef === 'undefined') {
        window.mudElementRef = {};
    }

    // Polyfill for addOnBlurEvent
    if (typeof window.mudElementRef.addOnBlurEvent !== 'function') {
        window.mudElementRef.addOnBlurEvent = function (element, dotNetRef) {
            if (!element) return;
            
            element.addEventListener('blur', function (e) {
                try {
                    if (dotNetRef && typeof dotNetRef.invokeMethodAsync === 'function') {
                        dotNetRef.invokeMethodAsync('OnBlur');
                    }
                } catch (error) {
                    console.warn('MudBlazor polyfill: Error invoking OnBlur', error);
                }
            });
        };
    }

    // Polyfill for other potentially missing functions
    if (typeof window.mudElementRef.addOnFocusEvent !== 'function') {
        window.mudElementRef.addOnFocusEvent = function (element, dotNetRef) {
            if (!element) return;
            
            element.addEventListener('focus', function (e) {
                try {
                    if (dotNetRef && typeof dotNetRef.invokeMethodAsync === 'function') {
                        dotNetRef.invokeMethodAsync('OnFocus');
                    }
                } catch (error) {
                    console.warn('MudBlazor polyfill: Error invoking OnFocus', error);
                }
            });
        };
    }

    // Additional MudBlazor functions that might need polyfilling
    if (typeof window.mudElementRef.saveFocus !== 'function') {
        window.mudElementRef.saveFocus = function (element) {
            if (element && typeof element.focus === 'function') {
                try {
                    element.focus();
                } catch (error) {
                    console.warn('MudBlazor polyfill: Error saving focus', error);
                }
            }
        };
    }

    if (typeof window.mudElementRef.select !== 'function') {
        window.mudElementRef.select = function (element) {
            if (element && typeof element.select === 'function') {
                try {
                    element.select();
                } catch (error) {
                    console.warn('MudBlazor polyfill: Error selecting element', error);
                }
            }
        };
    }

    if (typeof window.mudElementRef.selectRange !== 'function') {
        window.mudElementRef.selectRange = function (element, start, end) {
            if (element && typeof element.setSelectionRange === 'function') {
                try {
                    element.setSelectionRange(start, end);
                } catch (error) {
                    console.warn('MudBlazor polyfill: Error selecting range', error);
                }
            }
        };
    }

    console.log('✅ MudBlazor .NET 10 compatibility polyfill loaded successfully');
})();

