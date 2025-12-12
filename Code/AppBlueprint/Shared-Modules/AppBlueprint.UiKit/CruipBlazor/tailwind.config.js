/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./Components/**/*.{razor,html,cshtml}",
        "./Pages/**/*.{razor,html,cshtml}",
    ],
    safelist: [
        'dark:bg-gray-900',
        'dark:text-gray-400',
    ],
    theme: {
        extend: {},
    },
    plugins: [],
}
