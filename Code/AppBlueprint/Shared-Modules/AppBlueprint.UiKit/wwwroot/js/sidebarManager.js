"use strict";
/**
 * Sidebar management for desktop expand/collapse (UiKit Module)
 */
Object.defineProperty(exports, "__esModule", { value: true });
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
        if (body.classList.contains('sidebar-expanded')) {
            console.log('Sidebar already expanded');
        }
        else {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
            console.log('Sidebar expanded (was collapsed)');
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
    }
};
console.log('sidebarManager initialized:', typeof sidebarManager);
// Attach to globalThis for Blazor interop
globalThis.sidebarManager = sidebarManager;
exports.default = sidebarManager;
//# sourceMappingURL=sidebarManager.js.map