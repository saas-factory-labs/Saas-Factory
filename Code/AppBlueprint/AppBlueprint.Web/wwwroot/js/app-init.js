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

// Re-apply inline style= attributes that CSP's style-src blocks on the initial
// server-side rendered (SSR) HTML. Per the CSP3 spec, the restriction applies only
// to styles present in *serialized markup* — JavaScript-applied styles via the CSSOM
// (element.style.cssText) are not subject to style-src enforcement. This one-time
// pass runs on DOMContentLoaded so that Blazor Server components whose style=
// attributes are produced server-side (e.g. progress bars, avatar sizes) render
// correctly from the very first paint, before the Blazor circuit connects.
// All subsequent Blazor re-renders send style changes via JS DOM mutations, which
// are also CSP-exempt.
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('[style]').forEach(function (el) {
        var styleValue = el.getAttribute('style');
        if (styleValue) {
            el.style.cssText = styleValue;
        }
    });
});
