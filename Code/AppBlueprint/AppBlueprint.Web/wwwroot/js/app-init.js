// Initialize sidebar state for Cruip template
if (localStorage.getItem('sidebar-expanded') == 'true') {
    document.querySelector('body').classList.add('sidebar-expanded');
} else {
    document.querySelector('body').classList.remove('sidebar-expanded');
}

// Initialize dark mode from localStorage
// Default to light mode if no preference is stored
var savedTheme = localStorage.getItem('theme') || 'light';

if (savedTheme === 'dark') {
    document.documentElement.classList.add('dark');
    document.documentElement.style.colorScheme = 'dark';
    document.documentElement.style.backgroundColor = '#111827'; // gray-900
} else if (savedTheme === 'light') {
    document.documentElement.classList.remove('dark');
    document.documentElement.style.colorScheme = 'light';
    document.documentElement.style.backgroundColor = '#f3f4f6'; // gray-100
} else if (savedTheme === 'system') {
    // System preference
    if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
        document.documentElement.classList.add('dark');
        document.documentElement.style.colorScheme = 'dark';
        document.documentElement.style.backgroundColor = '#111827'; // gray-900
    } else {
        document.documentElement.classList.remove('dark');
        document.documentElement.style.colorScheme = 'light';
        document.documentElement.style.backgroundColor = '#f3f4f6'; // gray-100
    }
}
