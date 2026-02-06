/**
 * Sidebar management for desktop expand/collapse (UiKit Module)
 */
interface SidebarManager {
    toggleExpanded(): void;
    expand(): void;
    collapse(): void;
    isExpanded(): boolean;
}
declare global {
    var sidebarManager: SidebarManager;
}
declare const sidebarManager: SidebarManager;
export default sidebarManager;
//# sourceMappingURL=sidebarManager.d.ts.map