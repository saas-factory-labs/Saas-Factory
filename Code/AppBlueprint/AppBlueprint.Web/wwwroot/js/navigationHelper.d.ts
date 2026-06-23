/**
 * Navigation helper functions for Blazor interop
 */
interface NavigationHelper {
    navigateTo(url: string): void;
}
declare const navigationHelper: NavigationHelper;
