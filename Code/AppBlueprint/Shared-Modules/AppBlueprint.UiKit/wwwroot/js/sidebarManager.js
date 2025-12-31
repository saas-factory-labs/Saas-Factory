// Sidebar management for desktop expand/collapse
window.sidebarManager = {
    toggleExpanded: function () {
        console.log('sidebarManager.toggleExpanded called');
        const body = document.querySelector('body');
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
    
    expand: function () {
        console.log('sidebarManager.expand called');
        const body = document.querySelector('body');
        if (!body.classList.contains('sidebar-expanded')) {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
            console.log('Sidebar expanded (was collapsed)');
        } else {
            console.log('Sidebar already expanded');
        }
    },
    
    collapse: function () {
        console.log('sidebarManager.collapse called');
        const body = document.querySelector('body');
        if (body.classList.contains('sidebar-expanded')) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
            console.log('Sidebar collapsed (was expanded)');
        } else {
            console.log('Sidebar already collapsed');
        }
    },
    
    isExpanded: function () {
        const expanded = document.querySelector('body').classList.contains('sidebar-expanded');
        console.log('sidebarManager.isExpanded:', expanded);
        return expanded;
    }
};

console.log('sidebarManager initialized:', typeof window.sidebarManager);
