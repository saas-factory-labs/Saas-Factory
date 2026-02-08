/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './**/*.{razor,html,cshtml}',
    './Components/**/*.{razor,html}',
    './Pages/**/*.{razor,html}',
    './Shared/**/*.{razor,html}',
    './wwwroot/**/*.html'
  ],
  theme: {
    extend: {
      colors: {
        'brand-orange': '#ff6b35',
        'brand-dark': '#1a1a1a',
      },
      fontFamily: {
        'sans': ['Inter', 'sans-serif'],
      }
    }
  },
  plugins: [],
}
