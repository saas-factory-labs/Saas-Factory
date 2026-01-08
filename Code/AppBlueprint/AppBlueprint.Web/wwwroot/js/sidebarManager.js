"use strict";
/**
 * Sidebar management for desktop expand/collapse
 */
Object.defineProperty(exports, "__esModule", { value: true });
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
window.sidebarManager = sidebarManager;
exports.default = sidebarManager;
//# sourceMappingURL=sidebarManager.js.map