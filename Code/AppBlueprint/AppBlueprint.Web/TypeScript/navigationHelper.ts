/**
 * Navigation helper functions for Blazor interop
 */

interface NavigationHelper {
    navigateTo(url: string): void;
}

const navigationHelper: NavigationHelper = {
    navigateTo(url: string): void {
        globalThis.location.href = url;
    }
};

// Attach to window for Blazor interop
(globalThis as any).navigationHelper = navigationHelper;

