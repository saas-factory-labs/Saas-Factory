/**
 * Sidebar management for desktop expand/collapse (UiKit Module)
 */
interface SidebarManager {
    toggleExpanded(): void;
    expand(): void;
    collapse(): void;
    isExpanded(): boolean;
}
declare const sidebarManager: SidebarManager;
declare global {
    interface Window {
        sidebarManager: SidebarManager;
    }
}
export default sidebarManager;
//# sourceMappingURL=sidebarManager.d.ts.map