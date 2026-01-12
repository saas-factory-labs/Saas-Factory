/**
 * Navigation helper functions for Blazor interop
 */

interface NavigationHelper {
    navigateTo(url: string): void;
}

const navigationHelper: NavigationHelper = {
    navigateTo(url: string): void {
        window.location.href = url;
    }
};

// Attach to window for Blazor interop
(window as any).navigationHelper = navigationHelper;
