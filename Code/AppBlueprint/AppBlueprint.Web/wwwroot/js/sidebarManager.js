// Sidebar management for desktop expand/collapse
window.sidebarManager = {
    toggleExpanded: function () {
        const body = document.querySelector('body');
        const isExpanded = body.classList.contains('sidebar-expanded');
        
        if (isExpanded) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
        } else {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
        }
    },
    
    expand: function () {
        const body = document.querySelector('body');
        if (!body.classList.contains('sidebar-expanded')) {
            body.classList.add('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'true');
        }
    },
    
    collapse: function () {
        const body = document.querySelector('body');
        if (body.classList.contains('sidebar-expanded')) {
            body.classList.remove('sidebar-expanded');
            localStorage.setItem('sidebar-expanded', 'false');
        }
    },
    
    isExpanded: function () {
        return document.querySelector('body').classList.contains('sidebar-expanded');
    }
};
