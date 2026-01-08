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
declare global {
    interface Window {
        navigationHelper: NavigationHelper;
    }
}

window.navigationHelper = navigationHelper;

export default navigationHelper;
