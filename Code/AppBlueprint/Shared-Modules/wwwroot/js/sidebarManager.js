/**
 * Sidebar management for desktop expand/collapse (UiKit Module)
 */
const sidebarManager = {
    toggleExpanded() {
        console.log('sidebarManager.toggleExpanded called');
        const body = document.querySelector('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        const isExpanded = body.classList.contains('sidebar-expanded');
        if (isExpanded) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
            console.log('Sidebar collapsed');
        }
        else {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
            console.log('Sidebar expanded');
        }
    },
    expand() {
        console.log('sidebarManager.expand called');
        const body = document.querySelector('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        if (!body.classList.contains('sidebar-expanded')) {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
            console.log('Sidebar expanded (was collapsed)');
        }
        else {
            console.log('Sidebar already expanded');
        }
    },
    collapse() {
        console.log('sidebarManager.collapse called');
        const body = document.querySelector('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        if (body.classList.contains('sidebar-expanded')) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
            console.log('Sidebar collapsed (was expanded)');
        }
        else {
            console.log('Sidebar already collapsed');
        }
    },
    isExpanded() {
        const body = document.querySelector('body');
        if (body === null) {
            console.error('Body element not found');
            return false;
        }
        const expanded = body.classList.contains('sidebar-expanded');
        console.log('sidebarManager.isExpanded:', expanded);
        return expanded;
    },
    isAvailable() {
        return typeof window.sidebarManager !== 'undefined' &&
            typeof window.sidebarManager.expand === 'function' &&
            typeof window.sidebarManager.collapse === 'function';
    }
};
console.log('sidebarManager initialized:', typeof sidebarManager);
window.sidebarManager = sidebarManager;
export default sidebarManager;
//# sourceMappingURL=sidebarManager.js.map