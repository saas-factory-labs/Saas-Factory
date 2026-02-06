/**
 * Sidebar management for desktop expand/collapse
 */
const sidebarManager = {
    toggleExpanded() {
        const body = document.querySelector('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        const isExpanded = body.classList.contains('sidebar-expanded');
        if (isExpanded) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
        }
        else {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
        }
    },
    expand() {
        const body = document.querySelector('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        if (!body.classList.contains('sidebar-expanded')) {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
        }
    },
    collapse() {
        const body = document.querySelector('body');
        if (body === null) {
            console.error('Body element not found');
            return;
        }
        if (body.classList.contains('sidebar-expanded')) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
        }
    },
    isExpanded() {
        const body = document.querySelector('body');
        if (body === null) {
            console.error('Body element not found');
            return false;
        }
        return body.classList.contains('sidebar-expanded');
    }
};
globalThis.sidebarManager = sidebarManager;
export default sidebarManager;
//# sourceMappingURL=sidebarManager.js.map