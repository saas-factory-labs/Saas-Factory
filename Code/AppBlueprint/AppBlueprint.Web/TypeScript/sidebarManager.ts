/**
 * Sidebar management for desktop expand/collapse
 */

interface SidebarManager {
    toggleExpanded(): void;
    expand(): void;
    collapse(): void;
    isExpanded(): boolean;
}

const sidebarManager: SidebarManager = {
    toggleExpanded(): void {
        const body = document.querySelector<HTMLBodyElement>('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        
        const isExpanded = body.classList.contains('sidebar-expanded');
        
        if (isExpanded) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
        } else {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
        }
    },
    
    expand(): void {
        const body = document.querySelector<HTMLBodyElement>('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        
        if (!body.classList.contains('sidebar-expanded')) {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
        }
    },
    
    collapse(): void {
        const body = document.querySelector<HTMLBodyElement>('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        
        if (body.classList.contains('sidebar-expanded')) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
        }
    },
    
    isExpanded(): boolean {
        const body = document.querySelector<HTMLBodyElement>('body');
        if (body === null) {
            console.error('Body element not found');
            return false;
        }
        return body.classList.contains('sidebar-expanded');
    }
};

// Attach to window for Blazor interop
declare global {
    interface Window {
        sidebarManager: SidebarManager;
    }
}

window.sidebarManager = sidebarManager;

export default sidebarManager;
