/**
 * Sidebar management for desktop expand/collapse (UiKit Module)
 */

interface SidebarManager {
    toggleExpanded(): void;
    expand(): void;
    collapse(): void;
    isExpanded(): boolean;
    isAvailable(): boolean;
}

const sidebarManager: SidebarManager = {
    toggleExpanded(): void {
        console.log('sidebarManager.toggleExpanded called');
        const body = document.querySelector<HTMLBodyElement>('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        
        const isExpanded = body.classList.contains('sidebar-expanded');
        
        if (isExpanded) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
            console.log('Sidebar collapsed');
        } else {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
            console.log('Sidebar expanded');
        }
    },
    
    expand(): void {
        console.log('sidebarManager.expand called');
        const body = document.querySelector<HTMLBodyElement>('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        
        if (body.classList.contains('sidebar-expanded')) {
            console.log('Sidebar already expanded');
        } else {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
            console.log('Sidebar expanded (was collapsed)');
        }
    },
    
    collapse(): void {
        console.log('sidebarManager.collapse called');
        const body = document.querySelector<HTMLBodyElement>('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        
        if (body.classList.contains('sidebar-expanded')) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
            console.log('Sidebar collapsed (was expanded)');
        } else {
            console.log('Sidebar already collapsed');
        }
    },

    isExpanded(): boolean {
        const body = document.querySelector<HTMLBodyElement>('body');
        if (body === null) {
            console.error('Body element not found');
            return false;
        }
        const expanded = body.classList.contains('sidebar-expanded');
        console.log('sidebarManager.isExpanded:', expanded);
        return expanded;
    },

    isAvailable(): boolean {
        return globalThis.sidebarManager !== undefined && 
               globalThis.sidebarManager.expand !== undefined &&
               globalThis.sidebarManager.collapse !== undefined;
    }
};

console.log('sidebarManager initialized:', typeof sidebarManager);

// Attach to globalThis for Blazor interop
declare global {
    var sidebarManager: SidebarManager;
}

globalThis.sidebarManager = sidebarManager;

export default sidebarManager;
