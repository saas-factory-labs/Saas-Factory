/**
 * Navigation helper functions for Blazor interop
 */
interface NavigationHelper {
    navigateTo(url: string): void;
}
declare const navigationHelper: NavigationHelper;
declare global {
    interface Window {
        navigationHelper: NavigationHelper;
    }
}
export default navigationHelper;
//# sourceMappingURL=navigationHelper.d.ts.map